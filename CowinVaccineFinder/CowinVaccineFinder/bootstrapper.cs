using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CowinVaccineFinder
{
    class bootstrapper
    {
        public Main InitializeApplication(AppConfig configuration)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<ITelegramHelper, TelegramHelper>()
                .AddSingleton<ICowinService, CowinService>()
                .AddSingleton<IRestHelper, RestHelper>()
                .AddSingleton(typeof(AppConfig), configuration)
                .AddSingleton<Main>()
                .BuildServiceProvider();


            SetupLogging();

            Logger.GetLogger<bootstrapper>().Info("Starting application");
            return serviceProvider.GetService<Main>();
        }

        private void SetupLogging()
        {
            log4net.Config.BasicConfigurator.Configure();
        }

        private void SetupConfigurer()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables();

            var config = builder.Build();
        }
    }
}
