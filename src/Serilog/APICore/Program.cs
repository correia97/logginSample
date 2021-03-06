﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;

namespace APICore
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
            var db = documentClient.GetDatabase("log");

            Log.Logger = new LoggerConfiguration()
                  .MinimumLevel.Debug()
                  .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                  .Enrich.WithProperty("App Name", "API Core")
                  .Enrich.FromLogContext()
                  .WriteTo.Logger(lc => lc.Filter.ByExcluding(Matching.WithProperty("AuditLog"))
                                  .WriteTo.Console(new ElasticsearchJsonFormatter())
                                   .Enrich.WithThreadName()

                                  //.WriteTo.MongoDB(db,collectionName:"apiFull")
                                  .WriteTo.Debug()
                                  .WriteTo.File(@"d:\log\serilog\logCore.txt", fileSizeLimitBytes: 1_000_000,
                                                                               rollOnFileSizeLimit: true, shared: true,
                                                                               flushToDiskInterval: TimeSpan.FromSeconds(1))
                  )
                  .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(Matching.WithProperty("AuditLog"))
                                  .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                                  {
                                      AutoRegisterTemplate = true,
                                      AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                                      //PipelineName = "ApiCoreTestPipeline",
                                      //TypeName = "ApiCoreTesType"

                                  })
                                  .WriteTo.MongoDB(db, collectionName: "logCore")
                  )
                  .CreateLogger();
            try
            {
                Log.Information("Starting API Core Serilog");

                CreateHostBuilder(args).Build().Run();
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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
              Host.CreateDefaultBuilder(args)
                  .ConfigureWebHostDefaults(webBuilder =>
                  {
                      webBuilder.UseStartup<Startup>();
                  });
    }
}
