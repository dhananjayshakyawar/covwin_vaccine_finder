using Newtonsoft.Json;

namespace CowinVaccineFinder
{
    public class District
    {

        [JsonProperty("district_id")]
        public int Id { get; set; }

        [JsonProperty("state_id")]
        public int StateId { get; set; }

        [JsonProperty("district_name")]
        public string Name { get; set; }
    }
}