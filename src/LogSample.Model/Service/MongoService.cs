using LogSample.Model.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LogSample.Model.Service
{
    public class MongoService<T> : IMongoService<T> where T : class
    {
        private string MongoConnection { get; set; }
        private string MongoBase { get; set; }
        private IHttpContextAccessor httpContext { get; set; }
        private MongoClient Client { get; set; }
        private IMongoDatabase mongoDatabase { get; set; }
        private readonly string collectionName = typeof(T).Name;


        public MongoService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor;
            var mongoConfig = config.GetSection("Mongo");
            MongoConnection = mongoConfig["Connection"];
            MongoBase = mongoConfig["DataBase"];
            Client = new MongoClient(MongoConnection);
            mongoDatabase = Client.GetDatabase(MongoBase);
        }


        public async Task<LogModel<T>> GetById(object id)
        {
            try
            {
                var collection = mongoDatabase.GetCollection<LogModel<T>>(collectionName);

                var builder = Builders<LogModel<T>>.Filter;
                var filter = builder.Eq("ObjectId", $"{id.ToString()}");

                var result = await collection.FindAsync(filter);
                return await result.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<bool> Register(LogItem<T> log)
        {
            try
            {
                log.Url = httpContext.HttpContext.Request.Path.Value;

                var logModel = new LogModel<T>(log.User, log.OldData);
                logModel.History.Add(log);
                var collection = mongoDatabase.GetCollection<LogModel<T>>(collectionName);
                await collection.InsertOneAsync(logModel);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> Register(LogItem<T> log, object id)
        {
            try
            {
                log.Url = httpContext.HttpContext.Request.Path.Value;

                var logModel = new LogModel<T>(log.User, log.OldData);
                logModel.History.Add(log);
                logModel.ObjectId = id.ToString();
                var collection = mongoDatabase.GetCollection<LogModel<T>>(collectionName);
                await collection.InsertOneAsync(logModel);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> RegisterOrUpdate(LogItem<T> log, object id, [CallerMemberName] string memberName = "", [CallerFilePath] string memberFile = "")
        {
            try
            {
                log.Url = httpContext.HttpContext.Request.Path.Value;
                log.Method = memberName;
                log.File = memberFile;

                var logModel = await GetById( id);
                var exist = logModel != null;
                if (!exist)
                    logModel = new LogModel<T>(log.User, log.OldData);


                logModel.History.Add(log);
                logModel.ObjectId = id.ToString();

                var collection = mongoDatabase.GetCollection<LogModel<T>>(collectionName);
                if (exist)
                {
                    var builder = Builders<LogModel<T>>.Filter;
                    var filter = builder.Eq("ObjectId", $"{id}");
                    var result = await collection.ReplaceOneAsync(filter, logModel);
                    return result.ModifiedCount > 0;
                }
                else
                {
                    await collection.InsertOneAsync(logModel);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("--------------------------------------------------------");
                Debug.Print("--------------------------------------------------------");
                Debug.Print("--------------------------------------------------------");

                Debug.WriteLine("--------------------------------------------------------");
                Debug.WriteLine("--------------------------------------------------------");
                Debug.WriteLine("----------------------Exception----------------------------------");
                Debug.WriteLine(ex);
                Debug.WriteLine("--------------------------------------------------------");
                Debug.WriteLine("--------------------------------------------------------");
                Debug.WriteLine("-----------------------Exception message---------------------------------");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("--------------------------------------------------------");
                Debug.WriteLine("--------------------------------------------------------");
                Debug.WriteLine("--------------------------Stack trace------------------------------");
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine("--------------------------------------------------------");
                Debug.WriteLine("--------------------------------------------------------");
                Debug.WriteLine("-------------------------Inner exception-------------------------------");
                Debug.WriteLine(ex?.InnerException);
                Debug.WriteLine("--------------------------------------------------------");
                Debug.WriteLine("--------------------------------------------------------");
                Debug.WriteLine("-------------------------Inner Exception message-------------------------------");
                Debug.WriteLine(ex?.InnerException?.Message);

                return false;
            }
        }
    }
}
