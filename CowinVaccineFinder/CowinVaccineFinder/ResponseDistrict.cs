using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CowinVaccineFinder
{
    class ResponseDistrict
    {
        [JsonProperty("districts")]
        public IEnumerable<District> Districts { get; set; }
        [JsonProperty("ttl")]
        public int Ttl { get; set; }
    }
}
