using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.WinForm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            //Application.Run(new FormMain());

            /// https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            /// Dependency Injection using Generic HostBuilder in .NET Core Windows Form
            /// https://www.thecodebuzz.com/dependency-injection-net-core-windows-form-generic-hostbuilder/
            IHost host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<FormMain>();
                    services.AddSingleton<TT.GeneralHelper>();
                    services.AddSingleton<TT.Logging>();
                    //services.AddSingleton<TT.TcpSocket.Server>();
                }).Build();

            TT.Logging tLogger = host.Services.GetRequiredService<TT.Logging>();
            tLogger.ContentFormat = config.GetValue<string>(Param.AppSettings.TT.Logging.ContentFormat);
            tLogger.FilePathFormat = config.GetValue<string>(Param.AppSettings.TT.Logging.FilePathFormat);

            Param.Login.MaxRetry = config.GetValue<int>(Param.AppSettings.Login.MaxRetry);
            Param.Login.FailMessage = config.GetValue<string>(Param.AppSettings.Login.FailMessage);
            Param.Login.ExceedMaxRetryMessage = config.GetValue<string>(Param.AppSettings.Login.ExceedMaxRetryMessage);

            TT.TcpSocket.Client.Logger = tLogger;
            TT.TcpSocket.Server.Logger = tLogger;

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                try
                {
                    Application.Run(services.GetRequiredService<FormMain>());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}