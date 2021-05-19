using Telegram.Bot.Types;

namespace CowinVaccineFinder
{
    public class AppConfig
    {
        public SmtpOptions Smtp { get; set; }
        public MassTransitOptions MassTransit { get; set; }
        public string ConnectionString { get; set; }
        public string ResourceCalendarByDistrict { get; set; }
        public string ResourceDistrictFormat { get; set; }
        public string ResourceDistrictCalendarFormat { get; set; }
        public string ResourceStates { get; set; }
        public string CovinAPI { get; set; }
        public string FilterJsonFile { get;  set; }
        public int FilterMinAge { get;  set; }
        public string TelegramChatId { get;  set; }
        public string TelegramApiToken { get;  set; }
        public int WaitSeconds { get;  set; }
    }
}
