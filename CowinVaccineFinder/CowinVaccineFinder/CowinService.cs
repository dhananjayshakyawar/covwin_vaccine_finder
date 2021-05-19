using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CowinVaccineFinder
{
    class CowinService: ICowinService
    {
        private readonly IRestHelper restClient;
        private readonly ILog logger;
        private readonly AppConfig config;

        public CowinService(IRestHelper restClient, 
                            AppConfig config)
        {
            this.restClient = restClient;
            this.logger = Logger.GetLogger<CowinService>();
            this.config = config;
        }

        public IEnumerable<District> GetAllDistrictsByState(State state)
        {
            try
            {
                logger.Info(string.Format("Fetching Districts for State {0}-{1} ...",state.StateName, state.StateId));
                var request = new RestRequest(string.Format(config.ResourceDistrictFormat, state.StateId), Method.GET);
                IRestResponse response = restClient.GetRestClient.Execute(request);
                var data = JsonConvert.DeserializeObject<ResponseDistrict>(response.Content);
                logger.Info(string.Format("Received {0} districts for {1}",data.Districts.Count(), state.StateName));
                return data.Districts;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }

        public IEnumerable<State> GetAllStates()
        {
            try
            {
                logger.Info("Fetching States...");
                var request = new RestRequest(config.ResourceStates, Method.GET);
                IRestResponse response = restClient.GetRestClient.Execute(request);
                var data = JsonConvert.DeserializeObject<ResponseState>(response.Content);
                return data.States;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }

        }

        public IEnumerable<CovidCenter> GetDistrictSchedule(District district, DateTime startDate)
        {
            try
            {
                logger.Info(string.Format("Fetching Schedule for {0} district starting from {1} ...", district.Name, startDate.ToString("dd-MM-yyyy")));
                var request = new RestRequest(string.Format(config.ResourceDistrictCalendarFormat, district.Id, startDate.ToString("dd-MM-yyyy")), Method.GET);
                IRestResponse response = restClient.GetRestClient.Execute(request);
                var data = JsonConvert.DeserializeObject<ResponseDistrictCalendar>(response.Content);
                return data.CovidCenters;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new List<CovidCenter>();
            }
        }
    }
}
