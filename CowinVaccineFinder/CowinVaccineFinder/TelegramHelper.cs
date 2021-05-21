using log4net;
using Newtonsoft.Json;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
            FetchChatIds();
        }


        public string GetChatId(string groupName)
        {
            if (chatGroups.ContainsKey(groupName))
                return chatGroups[groupName];

            logger.InfoFormat("Unable to find chat id for telegram groups: {0}", groupName);
            return string.Empty;
        }
        public void FetchChatIds()
        {


            var filteredDistricts = JsonConvert.DeserializeObject<FilterDistricts>
                        (System.IO.File.ReadAllText(config.FilterJsonFile)).Districts;

            foreach (var district in filteredDistricts)
            {
                chatGroups.Add(district.TelegramGroup, district.TelegramGroupId);
            }

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