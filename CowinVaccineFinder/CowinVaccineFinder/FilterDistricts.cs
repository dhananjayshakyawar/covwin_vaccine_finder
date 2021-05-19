using Newtonsoft.Json;
using System.Collections.Generic;

namespace CowinVaccineFinder
{
    class FilterDistricts
    {
        [JsonProperty("districts")]
        public IEnumerable<FilterDistrict> Districts { get; set; }
    }
}
