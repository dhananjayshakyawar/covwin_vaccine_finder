using Newtonsoft.Json;

namespace CowinVaccineFinder
{
    public class TelegramChatUpdate
    {
        [JsonProperty("update_id")]
        public string UpdateId { get; set; }

        [JsonProperty("my_chat_member")]
        public TelegramChat ChatMembers { get; set; }
    }
}