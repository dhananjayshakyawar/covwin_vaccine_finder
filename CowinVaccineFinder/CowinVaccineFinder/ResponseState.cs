using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CowinVaccineFinder
{
    class ResponseState
    {
        [JsonProperty("states")]
        public IEnumerable<State> States { get; set; }
        [JsonProperty("ttl")]
        public int Ttl { get; set; }
    }
}
