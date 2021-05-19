using Newtonsoft.Json;

namespace CowinVaccineFinder
{
    public class TelegramChat
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string GroupTitle { get; set; }

        [JsonProperty("type")]
        public string ChatType { get; set; }
    }
}