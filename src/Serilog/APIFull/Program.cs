using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;

namespace APIFull
{
    public class Program
    {
        private static Lazy<MongoClient> lazyClient = new Lazy<MongoClient>(InitializeDocumentClient);
        private static MongoClient documentClient => lazyClient.Value;

        private static MongoClient InitializeDocumentClient()
        {
            return new MongoClient(connectionString: "mongodb://localhost:27017/log");

        }
        public static void Main(string[] args)
        {
           // var db = documentClient.GetDatabase("log");

            Log.Logger = new LoggerConfiguration()
                  .MinimumLevel.Debug()
                  .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                  .Enrich.WithProperty("App Name", "API Full")
                  .Enrich.FromLogContext()
                  .WriteTo.Logger(lc => lc.Filter.ByExcluding(Matching.WithProperty("AuditLog"))
                                  .WriteTo.Console(new ElasticsearchJsonFormatter())
                                   .Enrich.WithThreadName()

                                  //.WriteTo.MongoDB(db,collectionName:"apiFull")
                                  .WriteTo.Debug()
                                  .WriteTo.File(@"d:\log\serilog\logFull.txt", fileSizeLimitBytes: 1_000_000,
                                                                               rollOnFileSizeLimit: true, shared: true,
                                                                               flushToDiskInterval: TimeSpan.FromSeconds(1))
                  )
                  .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(Matching.WithProperty("AuditLog"))
                                  .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://search-paulolog-skcdv722cchxumxwmb7qguy5tm.us-east-2.es.amazonaws.com/"))
                                  {
                                      AutoRegisterTemplate = true,
                                      AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,

                                  })
                  )
                  .CreateLogger();
            try
            {
                Log.Information("Starting API Full Serilog");
                CreateWebHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            .UseSerilog();
    }
}
