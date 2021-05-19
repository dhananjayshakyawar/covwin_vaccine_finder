using Newtonsoft.Json;
using System.Collections.Generic;

namespace CowinVaccineFinder
{
    public class CovidCenter
    {
        [JsonProperty("center_id")]
        public int Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("state_name")]
        public string State { get; set; }
        [JsonProperty("district_name")]
        public string District { get; set; }
        [JsonProperty("pincode")]
        public string PinCode { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("fee_type")]
        public string FeeType { get; set; }

        [JsonProperty("vaccine_fees")]
        public IEnumerable<VaccineFee> VaccineFees { get; set; }

        [JsonProperty("sessions")]
        public IEnumerable<VaccineSession> VaccineSessions { get; set; }
    }
}