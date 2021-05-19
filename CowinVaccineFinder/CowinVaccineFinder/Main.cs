using log4net;
using Newtonsoft.Json;
using System;
using Telegram.Bot;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace CowinVaccineFinder
{
    class Main
    {
        private readonly ILog logger;
        private readonly ICowinService cowinService;
        private readonly AppConfig config;
        private readonly ITelegramHelper telegram;

        public Main(ICowinService cowinService, AppConfig config, ITelegramHelper telegram)
        {
            this.logger = Logger.GetLogger<Main>();
            this.cowinService = cowinService;
            this.config = config;
            this.telegram = telegram;
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


                        //for each dis get schedule
                        foreach (var district in districts)
                        {
                            if (!(district.Name == destrictsToCheck.District))
                                continue;
                            var centers = cowinService.GetDistrictSchedule(district, DateTime.Now);

                            var availableCenters = centers.Where(x => (x.VaccineSessions.Count() > 0
                                                && x.VaccineSessions.Any
                                                (y => (y.CapacityDose2 > 0 &&
                                                y.MinimunAge == config.FilterMinAge))))
                                                .ToList();

                            foreach (var center in availableCenters)
                            {
                                foreach (var session in center.VaccineSessions)
                                {
                                    if  (!(session.MinimunAge == config.FilterMinAge && session.CapacityDose1 > 0))
                                        continue;

                                    var msg = string.Format("Vaccine available: {8}" +
                                    "\n\nHospital: {0}\n{7}\nPIN:{1}\n\nVaccine:{2}\nFees:{3}" +
                                    "\nAge:{4}+\n\nDose1 available: {5}\nDose2 available: {6}",
                                    center.Name,
                                    center.PinCode,
                                    session.Vaccine,
                                    center.VaccineFees.Single(x=> x.Vaccine == session.Vaccine).Fee,
                                    session.MinimunAge,
                                    session.CapacityDose1,
                                    session.CapacityDose2,
                                    center.District,
                                    session.Date);
                                    var tsk = telegram.SendMessageAsync(msg);

                                    tasks.Add(tsk);
                                    updateSent = true;
                                }
                            }



                            //check available
                        }

                    }
                    //send comms
                    Task.WaitAll(tasks.ToArray());
                }
                var wait = updateSent?config.WaitSeconds:30000;
                Thread.Sleep(wait);
            }
        }
    }
}
