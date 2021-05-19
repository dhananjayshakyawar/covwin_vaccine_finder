using log4net;
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
        public TelegramHelper(AppConfig config)
        {
            client = new TelegramBotClient(config.TelegramApiToken);
            this.config = config;
            this.logger = Logger.GetLogger<Main>();
        }
        public async System.Threading.Tasks.Task SendMessageAsync(string message)
        {
            try
            {
                logger.Info(string.Format("Sending message to Telegram: {0}", message));
                Message msg = await client.SendTextMessageAsync(
                  chatId: config.TelegramChatId,
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