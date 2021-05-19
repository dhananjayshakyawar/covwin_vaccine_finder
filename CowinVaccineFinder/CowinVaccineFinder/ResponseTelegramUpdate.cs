using Newtonsoft.Json;
using System.Collections.Generic;

namespace CowinVaccineFinder
{
    internal class ResponseTelegramUpdate
    {
        [JsonProperty("result")]
        public IEnumerable<TelegramChatUpdate> ChatUpdates { get; set; }
    }
}