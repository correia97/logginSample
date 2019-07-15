using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using MongoDB.Driver;
using NLog.Web;
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
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();


            try
            {
                logger.Debug("init Api Core NLog");
                CreateWebHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
        .UseNLog();
    }
}
