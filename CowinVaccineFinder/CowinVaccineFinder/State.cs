using Newtonsoft.Json;

namespace CowinVaccineFinder
{
    internal class State
    {
        [JsonProperty("state_id")]
        public int StateId { get; set; }

        [JsonProperty("state_name")]
        public string StateName { get; set; }
    }
}