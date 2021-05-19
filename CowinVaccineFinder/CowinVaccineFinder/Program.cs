using System;
using Microsoft.Extensions.Configuration;

namespace CowinVaccineFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var applcation = new bootstrapper();
            applcation.InitializeApplication(InitOptions<AppConfig>()).Run();

        }

        private static T InitOptions<T>()
        where T : new()
        {
            var config = InitConfig();
            return config.Get<T>();
        }

        private static IConfigurationRoot InitConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
