using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CowinVaccineFinder
{
    class Main
    {
        private readonly ILog logger;
        private readonly ICowinService cowinService;
        private readonly AppConfig config;
        private readonly ITelegramHelper telegram;
        private readonly string doseTrackerFormat = "{0},{1}";
        private Dictionary<string, string> DoseTracker;
        private int apiCallsCowin = 0;

        public Main(ICowinService cowinService, AppConfig config, ITelegramHelper telegram)
        {
            this.logger = Logger.GetLogger<Main>();
            this.cowinService = cowinService;
            this.config = config;
            this.telegram = telegram;
            DoseTracker = new Dictionary<string, string>();
        }

        public void Run()
        {
            logger.Info("Starting to pool ...");

            var filteredDistricts = JsonConvert.DeserializeObject<FilterDistricts>
                                    (System.IO.File.ReadAllText(config.FilterJsonFile)).Districts;
            var states = cowinService.GetAllStates();

            while (true)
            {

                var tasks = new List<Task>();

                foreach (var destrictsToCheck in filteredDistricts)
                {
                    var totalDoseForDistrict = 0;

                    foreach (var state in states)
                    {
                        if (!(state.StateName == destrictsToCheck.State))
                            continue;
                        //get districts
                        var districts = cowinService.GetAllDistrictsByState(state);


                        //for each dis get schedule
                        foreach (var district in districts)
                        {
                            if (!(district.Name == destrictsToCheck.District))
                                continue;
                            var centers = cowinService.GetDistrictSchedule(district, DateTime.Now.ToLocalTime());


                            var availableCenters = centers.Where(x => (x.VaccineSessions.Count() > 0
                                                && x.VaccineSessions.Any
                                                (y => (y.CapacityDose1 > 1 &&
                                                y.MinimunAge == config.FilterMinAge))))
                                                .ToList();
                            if (availableCenters.Count == 0)
                                logger.InfoFormat("No updates from API for: {0}", district.Name);

                            foreach (var center in availableCenters)
                            {
                                foreach (var session in center.VaccineSessions)
                                {
                                    if (!(session.MinimunAge == config.FilterMinAge && session.CapacityDose1 > 1))
                                        continue;

                                    var centerKey = string.Format("{0}/{1}", center.Id,session.Date);

                                    if (DoseTracker.ContainsKey(centerKey)
                                        && (DoseTracker[centerKey] ==
                                        string.Format(doseTrackerFormat,
                                                        session.MinimunAge,
                                                        session.CapacityDose1,
                                                        session.CapacityDose2)))
                                    {
                                        logger.InfoFormat("[{0}] Ignoring...No change. Previous Values  - {1}",
                                            center.District,
                                            DoseTracker[centerKey]);

                                        continue;
                                    }
                                    else if (DoseTracker.ContainsKey(centerKey)
                                            && Convert.ToInt32(DoseTracker[centerKey].Split(',')[1]) >= session.CapacityDose1)
                                    {
                                        logger.InfoFormat("[{0}] Ignoring...Dose reduced to {1} - previous value:{1}",
                                                            center.District,
                                                            session.CapacityDose1,
                                                            DoseTracker[centerKey]);

                                        DoseTracker[centerKey] = string.Format(doseTrackerFormat,
                                                                                    session.MinimunAge,
                                                                                    session.CapacityDose1,
                                                                                    session.CapacityDose2);
                                        continue;

                                    }
                                    else if (DoseTracker.ContainsKey(centerKey))
                                    {
                                        DoseTracker[centerKey] = string.Format(doseTrackerFormat,
                                                                                    session.MinimunAge,
                                                                                    session.CapacityDose1,
                                                                                    session.CapacityDose2);
                                    }
                                    else
                                    {
                                        DoseTracker.Add(centerKey, string.Format(doseTrackerFormat,
                                                                                    session.MinimunAge,
                                                                                    session.CapacityDose1,
                                                                                    session.CapacityDose2));
                                    }


                                    var dose = session.CapacityDose1 > 0 ? string.Format("Dose-1, Qty:{0} &", session.CapacityDose1) : string.Empty;
                                    dose += session.CapacityDose2 > 0 ? string.Format(" Dose-2, Qty:{0} &", session.CapacityDose2) : string.Empty;

                                    dose = dose.Substring(0, dose.Length - 2);

                                    var msg = string.Format("**{9}** available\nDate: {8}" +
                                    "\n\nHospital: {0}\n{7}\nPIN:{1}\n\nVaccine:{2}\nFees:{3}" +
                                    "\nAge:{4}+\n\nDose1 available: {5}\nDose2 available: {6}\n",
                                    center.Name,
                                    center.PinCode,
                                    session.Vaccine,
                                    center.FeeType == "Free" ? "Free" :
                                    center.VaccineFees.Single(x => x.Vaccine == session.Vaccine).Fee,
                                    session.MinimunAge,
                                    session.CapacityDose1,
                                    session.CapacityDose2,
                                    center.District,
                                    session.Date,
                                    dose);

                                    totalDoseForDistrict += session.CapacityDose1;

                                    logger.InfoFormat("Sending comms - {0} : {1}", destrictsToCheck.TelegramGroup, msg);

                                    var chatId = telegram.GetChatId(destrictsToCheck.TelegramGroup);
                                    var tsk = telegram.SendMessageAsync(msg, chatId);

                                    tasks.Add(tsk);
                                }
                            }

                            if (totalDoseForDistrict > 1)
                            {
                                var filepath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)? 
                                    "/var/www/html/data/{0}{1}.datapoints" 
                                    : "html/data/{0}{1}.datapoints";

                                System.IO.File.AppendAllText(string.Format(filepath, district.Name, DateTime.Now.ToLocalTime().ToString("dd-MM-yyyy"))
                                , string.Format("{0},{1}{2}", DateTime.Now.ToJavascriptTicks(),
                                                        totalDoseForDistrict,
                                                        Environment.NewLine));
                            }

                            //check available
                        }

                    }
                    //send comms
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();

                    //CheckApiThreashold();
                    Thread.Sleep(500);
                }

                Thread.Sleep((6 * filteredDistricts.Count() * 1000) + 1000);
                //CheckApiThreashold();
            }
        }
    }
}
