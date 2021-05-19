using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CowinVaccineFinder
{
    class RestHelper: IRestHelper
    {
        RestClient _client;
        public RestHelper(AppConfig config)
        {
            URL = config.CovinAPI;
            _client = new RestClient(URL);

        }

        public string URL { get; }
        public RestClient GetRestClient { get
            {
                return _client;
            }
        }

    }
}
