using Newtonsoft.Json;

namespace CowinVaccineFinder
{
    class FilterDistrict
    {
        [JsonProperty("state_name")]
        public string State { get; set; }

        [JsonProperty("district_name")]
        public string District { get; set; }
    }
}
