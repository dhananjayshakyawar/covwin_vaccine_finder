using Newtonsoft.Json;
using System.Collections.Generic;

namespace CowinVaccineFinder
{
    public class VaccineSession
    {
          
          
        [JsonProperty("session_id")]
        public string Id { get; set; }
            
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("available_capacity")]
        public int TotalCapacity { get; set; }

        [JsonProperty("available_capacity_dose1")]
        public int CapacityDose1 { get; set; }

        [JsonProperty("available_capacity_dose2")]
        public int CapacityDose2 { get; set; }

        [JsonProperty("min_age_limit")]
        public int MinimunAge { get; set; }

        [JsonProperty("vaccine")]
        public string Vaccine { get; set; }

        [JsonProperty("slots")]
        public IEnumerable<string> Slots { get; set; }
    }
}