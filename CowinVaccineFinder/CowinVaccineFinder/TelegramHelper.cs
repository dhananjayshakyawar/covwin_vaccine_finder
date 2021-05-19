using log4net;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;

namespace CowinVaccineFinder
{
    internal class TelegramHelper : ITelegramHelper
    {
        private readonly AppConfig config;
        private readonly ILog logger;
        TelegramBotClient client;
        Dictionary<string, string> chatGroups;
        public TelegramHelper(AppConfig config)
        {
            client = new TelegramBotClient(config.TelegramApiToken);
            this.config = config;
            this.logger = Logger.GetLogger<Main>();
            chatGroups = new Dictionary<string, string>();

            chatGroups.Add("COWIN-18-PLUS-MH-THANE", "-1001207388460");
            chatGroups.Add("COWIN-18-PLUS-MH-MUMBAI", "-1001382711013");
            chatGroups.Add("COWIN-18-PLUS-MP-SHAHDOL", "-1001166172103");
            chatGroups.Add("COWIN-18-PLUS-MP-INDORE", "-1001162642870");
        }

        public string GetChatId(string groupName)
        {

            if (chatGroups.ContainsKey(groupName))
                return chatGroups[groupName];


            var restClient = new 
                RestClient(config.TelegramAPI);

            logger.Info(string.Format("Fetching Group ID for {0} ...", groupName));
            var request = new RestRequest(
                string.Format(config.TelegramGetUpdateResourceFormat
                , config.TelegramApiToken), Method.GET);

            IRestResponse response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                var data = JsonConvert.DeserializeObject<ResponseTelegramUpdate>(response.Content);

                var chat = data.ChatUpdates.Single(x => x.ChatMembers.GroupTitle == groupName);

                return chat.ChatMembers.Id;
            }

            return string.Empty;
        }
        public async System.Threading.Tasks.Task SendMessageAsync(string message, string chatId)
        {
            try
            {
                logger.Info(string.Format("Sending message to Telegram: {0}", message));
                Message msg = await client.SendTextMessageAsync(
                  chatId: chatId,
                  text: message,
                  parseMode: ParseMode.Markdown,
                  disableNotification: false,
                  //replyToMessageId: e.Message.MessageId,
                  replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                    "Click to visit COWIN",
                    "https://selfregistration.cowin.gov.in/dashboard"
                  ))
                );
            }
            catch (System.Exception e)
            {
                logger.Error(e);
            }


        }
    }
}