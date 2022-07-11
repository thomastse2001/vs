// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;
using AspNetCore6WebApp.Business;

Console.WriteLine("Start.");

/// https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

/// Setup the dependency injection.
/// https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) => services
    .AddLogging(loggingBuilder =>
    {
        var loggingSection = config.GetSection(Param.AppSettings.Logging);
        loggingBuilder.AddFile(loggingSection);
        //loggingBuilder.AddFile("ChatbotScheduler.log", append: true);
    })
    //.AddDbContextPool<AspNetCore6WebAppDbContext>(options =>
    //{
    //    /// Efficient updating by adjusting the minimum and maximum batch size.
    //    /// https://docs.microsoft.com/en-us/ef/core/performance/efficient-updating
    //    options.UseSqlServer(config.GetConnectionString("SQLServer"),
    //        o => o.MinBatchSize(config.GetValue<int>("DbMinBatchSize", 0)).MaxBatchSize(config.GetValue<int>("DbMaxBatchSize", 500)));
    //})
    .AddDbContextPool<AspNetCore6WebAppDbContext>(options =>
    {
        options.UseSqlite(string.Format(config.GetConnectionString(Param.AppSettings.ConnectionStrings.SQLite), config.GetValue<string>(Param.AppSettings.SqliteDatabaseFilePath)),
            o => o.MinBatchSize(config.GetValue<int>(Param.AppSettings.Console.DbMinBatchSize, 1)).MaxBatchSize(config.GetValue<int>(Param.AppSettings.Console.DbMaxBatchSize, 500)));
    })
    .AddSingleton<IProductDao, ProductDao>()
    .AddSingleton<IVendorDao, VendorDao>()
    .AddSingleton<TT.IDbHelper, TT.DbHelper>()
    .AddSingleton<TT.IExcelDataReaderHelper, TT.ExcelDataReaderHelper>()
    .AddSingleton<IBatchUpdateBo, BatchUpdateBo>()
    .AddSingleton<TT.Logging>()
    ).Build();

///// https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration
//IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

ILogger logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogWarning("AspNetCore6WebApp start.");

TT.Logging tlogger = host.Services.GetRequiredService<TT.Logging>();
tlogger.ContentFormat = config.GetValue<string>(Param.AppSettings.TT.Logging.ContentFormat);
tlogger.FilePathFormat = config.GetValue<string>(Param.AppSettings.TT.Logging.FilePathFormat);
tlogger.Debug("TLogger. {0:HH:mm:ss}", DateTime.Now);

// For ExcelDataReader.
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

ExemplifyScoping(config, logger, host.Services, "Scope 1");

await host.StartAsync();

logger.LogWarning("AspNetCore6WebApp end.");
Console.WriteLine("End.");

static void ExemplifyScoping(IConfiguration config, ILogger logger, IServiceProvider services, string name)
{
    using IServiceScope serviceScope = services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    /// Actual work.
    string sqliteDatabaseFilePath = config.GetValue<string>(Param.AppSettings.SqliteDatabaseFilePath);
    Console.WriteLine($"sqliteDatabaseFilePath = {sqliteDatabaseFilePath}");
    logger.LogWarning("sqliteDatabaseFilePath = {sqliteDatabaseFilePath}", sqliteDatabaseFilePath);

    string excelPath = config.GetValue<string>(Param.AppSettings.Console.DbExcelFile.Path);
    var batchUpdateBo = provider.GetRequiredService<IBatchUpdateBo>();
    batchUpdateBo.UpdateFromExcel<Product>(0, excelPath, config.GetValue<string>(Param.AppSettings.Console.DbExcelFile.Sheet.Product));
    batchUpdateBo.UpdateFromExcel<Vendor>(0, excelPath, config.GetValue<string>(Param.AppSettings.Console.DbExcelFile.Sheet.Vendor));
}
