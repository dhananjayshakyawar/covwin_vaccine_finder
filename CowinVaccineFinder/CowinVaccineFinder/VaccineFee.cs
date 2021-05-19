using Newtonsoft.Json;

namespace CowinVaccineFinder
{
    public class VaccineFee
    {
        [JsonProperty("vaccine")]
        public string Vaccine { get; set; }

        [JsonProperty("fee")]
        public string  Fee { get; set; }
    }
}