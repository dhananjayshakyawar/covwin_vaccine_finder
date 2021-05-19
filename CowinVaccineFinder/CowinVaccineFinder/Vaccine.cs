using Newtonsoft.Json;

namespace CowinVaccineFinder
{
    public class Vaccine
    {
        [JsonProperty("vaccine")]
        public string Name { get; set; }

        [JsonProperty("fee")]
        public int Fee { get; set; }
    }
}