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
                logger.Info(string.Format("[HEADER] Fetching Districts for State {0}-{1} ...",state.StateName, state.StateId));
                var request = new RestRequest(string.Format(config.ResourceDistrictFormat, state.StateId), Method.GET);
                request.AddHeader("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36");
                IRestResponse response = restClient.GetRestClient.Execute(request);

                if(!response.IsSuccessful)
                {
                    logger.WarnFormat("Response failed - {0}", response.StatusCode.ToString());
                    return new List<District>();
                }

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
                logger.Info("[HEADER] Fetching States...");
                var request = new RestRequest(config.ResourceStates, Method.GET);
                request.AddHeader("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36");
                IRestResponse response = restClient.GetRestClient.Execute(request);
                if (!response.IsSuccessful)
                {
                    logger.ErrorFormat("Request Failed - {0}", response.StatusCode);
                    logger.Error(response);
                    return new List<State>();
                }
                    
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
                logger.Info(string.Format("[HEADER]  Fetching Schedule for {0} district starting from {1} ...", district.Name, startDate.ToString("dd-MM-yyyy")));
                var request = new RestRequest(string.Format(config.ResourceDistrictCalendarFormat, district.Id, startDate.ToString("dd-MM-yyyy")), Method.GET);
                request.AddHeader("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36");
                IRestResponse response = restClient.GetRestClient.Execute(request);
                if (!response.IsSuccessful)
                {
                    logger.WarnFormat("Response failed - {0}", response);
                    return new List<CovidCenter>();
                }
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
