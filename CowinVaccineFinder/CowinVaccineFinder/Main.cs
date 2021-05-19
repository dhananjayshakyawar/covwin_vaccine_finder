using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly string doseTrackerFormat = "{0},{1},{2}";
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
                var updateSent = false;

                foreach (var destrictsToCheck in filteredDistricts)
                {

                    foreach (var state in states)
                    {
                        if (!(state.StateName == destrictsToCheck.State))
                            continue;
                        //get districts
                        var districts = cowinService.GetAllDistrictsByState(state);
                        apiCallsCowin += 1;


                        //for each dis get schedule
                        foreach (var district in districts)
                        {
                            if (!(district.Name == destrictsToCheck.District))
                                continue;
                            var centers = cowinService.GetDistrictSchedule(district, DateTime.Now);
                            apiCallsCowin += 1;


                            var availableCenters = centers.Where(x => (x.VaccineSessions.Count() > 0
                                                && x.VaccineSessions.Any
                                                (y => (y.TotalCapacity > 0 &&
                                                y.MinimunAge == config.FilterMinAge))))
                                                .ToList();
                            if (availableCenters.Count == 0)
                                logger.InfoFormat("No updates from API for: {0}", district.Name);

                            foreach (var center in availableCenters)
                            {
                                foreach (var session in center.VaccineSessions)
                                {
                                    if  (!(session.MinimunAge == config.FilterMinAge && session.TotalCapacity > 0))
                                        continue;

                                    if (DoseTracker.ContainsKey(center.PinCode)
                                        && (DoseTracker[center.PinCode] ==  
                                        string.Format(doseTrackerFormat,
                                                        session.MinimunAge,
                                                        session.CapacityDose1,
                                                        session.CapacityDose2)))
                                    {
                                        logger.InfoFormat("Ignoring...No change for {0} - {1}",
                                            center.District,
                                            DoseTracker[center.PinCode]);

                                        continue;
                                    }
                                    else if (DoseTracker.ContainsKey(center.PinCode))
                                    {
                                        DoseTracker[center.PinCode] = string.Format(doseTrackerFormat,
                                                                                    session.MinimunAge,
                                                                                    session.CapacityDose1, 
                                                                                    session.CapacityDose2);
                                    }
                                    else
                                    {
                                        DoseTracker.Add(center.PinCode, string.Format(doseTrackerFormat,
                                                                                    session.MinimunAge,
                                                                                    session.CapacityDose1,
                                                                                    session.CapacityDose2));
                                    }


                                    var dose = session.CapacityDose1 > 0 ? "Dose-1 &":string.Empty;
                                    dose += session.CapacityDose2 > 0 ? "Dose-2 &": string.Empty;

                                    dose = dose.Substring(0, dose.Length - 2);

                                    var msg = string.Format("**{9}** available: {8}" +
                                    "\n\nHospital: {0}\n{7}\nPIN:{1}\n\nVaccine:{2}\nFees:{3}" +
                                    "\nAge:{4}+\n\nDose1 available: {5}\nDose2 available: {6}",
                                    center.Name,
                                    center.PinCode,
                                    session.Vaccine,
                                    center.FeeType == "Free"? "Free": 
                                    center.VaccineFees.Single(x=> x.Vaccine == session.Vaccine).Fee,
                                    session.MinimunAge,
                                    session.CapacityDose1,
                                    session.CapacityDose2,
                                    center.District,
                                    session.Date,
                                    dose);

                                    logger.InfoFormat("Sending comms - {0} : {1}", destrictsToCheck.TelegramGroup, msg);
                                    var chatId = telegram.GetChatId(destrictsToCheck.TelegramGroup);
                                    var tsk = telegram.SendMessageAsync(msg, chatId);

                                    tasks.Add(tsk);
                                    updateSent = true;
                                }
                            }



                            //check available
                        }

                    }
                    //send comms
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();

                    //CheckApiThreashold();
                    Thread.Sleep(100);
                }

                Thread.Sleep(60000);
                //CheckApiThreashold();
            }
        }
        private void CheckApiThreashold()
        {
            if (apiCallsCowin >= 95)
            {
                logger.InfoFormat("Reached call Limit - {0}. Wait for {1} millis",
                    apiCallsCowin,config.WaitSeconds);
                Thread.Sleep(config.WaitSeconds);
                apiCallsCowin = 0;
            }
        }
    }
}
