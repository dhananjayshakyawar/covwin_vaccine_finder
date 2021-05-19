using Newtonsoft.Json;

namespace CowinVaccineFinder
{
    class FilterDistrict
    {
        [JsonProperty("state_name")]
        public string State { get; set; }

        [JsonProperty("district_name")]
        public string District { get; set; }

        [JsonProperty("telegram_group")]
        public string TelegramGroup { get; set; }

        [JsonProperty("telegram_group_id")]
        public string TelegramGroupId { get; set; }

        [JsonProperty("min_age")]
        public int MinAge { get; set; }
    }
}
