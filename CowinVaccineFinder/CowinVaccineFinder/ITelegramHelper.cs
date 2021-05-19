namespace CowinVaccineFinder
{
    internal interface ITelegramHelper
    {
         System.Threading.Tasks.Task SendMessageAsync(string message);
    }
}