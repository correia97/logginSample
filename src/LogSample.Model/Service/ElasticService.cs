using Flurl;
using Flurl.Http;
using LogSample.Model.Interface;
using LogSample.Model.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LogSample.Model.Service
{
    public class ElasticService<T> : IElasticService<T> where T : class, ICloneable
    {
        private string ElasticUrl { get; set; }
        private HttpClient Client { get; set; }

        private IHttpContextAccessor httpContext { get; set; }
        private ElasticClient elasticsearchClient { get; set; }

        private readonly string collectionName = typeof(T).Name.ToLower();
        private AsyncRetryPolicy<HttpResponseMessage> Policy { get; set; }
        public ElasticService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            FlurlHttp.Configure(s =>
            {
                s.JsonSerializer = new Flurl.Http.Configuration.NewtonsoftJsonSerializer(new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    Formatting = Formatting.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            });


            if (Client == null)
                Client = new HttpClient();
            ElasticUrl = config.GetSection("Elastic").Value;


            Client.BaseAddress = new Uri(config.GetSection("Elastic").Value);
            httpContext = httpContextAccessor;


            var node = new Uri(config.GetSection("Elastic").Value);
            var settings = new ConnectionSettings(node);
            settings.DefaultIndex(collectionName);
            elasticsearchClient = new ElasticClient(settings);
            HttpStatusCode[] httpStatusCodesWorthRetrying = {
                       HttpStatusCode.RequestTimeout, // 408
                       HttpStatusCode.InternalServerError, // 500
                       HttpStatusCode.BadGateway, // 502
                       HttpStatusCode.ServiceUnavailable, // 503
                       HttpStatusCode.GatewayTimeout // 504
                    };

            Policy = Polly.Policy.HandleResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode))
                                                .WaitAndRetryAsync(new[]
                                  {
                                    TimeSpan.FromSeconds(1),
                                    TimeSpan.FromSeconds(2),
                                    TimeSpan.FromSeconds(3)
                                  }, (exception, timeSpan, context) =>
                                  {
                                      Debug.WriteLine("-------------------------------------- exception ------------------------------------------");
                                      Debug.WriteLine(exception);
                                      Debug.WriteLine("-------------------------------------- timeSpan  ------------------------------------------");
                                      Debug.WriteLine(timeSpan);
                                      Debug.WriteLine("-------------------------------------- context   ------------------------------------------");
                                      Debug.WriteLine(context);
                                  });



        }
        public async Task<LogModel<T>> GetById( object id)
        {
            try
            {
                LogModel<T> result = null;
                var response = await Policy.ExecuteAsync(() => ElasticUrl.AllowAnyHttpStatus()
                                                                .AppendPathSegment($"{collectionName}/_doc/{id}")
                                                                .GetAsync());

                if (response.IsSuccessStatusCode)
                {
                    var temp = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(temp);
                    var data = JsonConvert.DeserializeObject<ElasticModel<LogModel<T>>>(temp);

                    result = data._source;
                }

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> Register(LogItem<T> item, object id)
        {
            var log = new LogModel<T>(item.User, item.OldData);
            log.History.Add(item);
            var result = await Policy.ExecuteAsync(() => ElasticUrl.AllowAnyHttpStatus()
                                                            .AppendPathSegment($"{collectionName}/_doc/{id}")
                                                            .PostJsonAsync(log));

            Debug.WriteLine(result.ReasonPhrase);

            var data = JsonConvert.DeserializeObject<ElasticModel<T>>(await result.Content.ReadAsStringAsync());
            Debug.WriteLine(data);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Register(LogItem<T> item)
        {
            var log = new LogModel<T>(item.User, item.OldData);
            log.History.Add(item);
            var result = await Policy.ExecuteAsync(() => ElasticUrl.AllowAnyHttpStatus()
                                                               .AppendPathSegment($"{collectionName}/_doc")
                                                           .PostJsonAsync(log));

#if DEBUG
            Debug.WriteLine(result.ReasonPhrase);

            Debug.WriteLine(await result.Content.ReadAsStringAsync());
#endif
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> RegisterOrUpdate(LogItem<T> item, object id, string memberName, string memberFile)
        {
            item.Url = httpContext.HttpContext.Request.Path.Value;
            item.Method = memberName;
            item.File = memberFile;

            var log = await GetById(id);
            if (log != null)
            {
                log.History.Add(item);
            }
            else
            {
                log = new LogModel<T>(item.User, item.OldData);
                log.History.Add(item);
            }

            //var message = new HttpRequestMessage(HttpMethod.Put, $"{objectName.ToLower()}/_doc/{id}");
            //message.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            var result = await Policy.ExecuteAsync(() => ElasticUrl.AllowAnyHttpStatus()
                                                            .AppendPathSegment($"{collectionName}/_doc/{id}")
                                                            .PutJsonAsync(log));

            Debug.WriteLine(result.ReasonPhrase);

            Debug.WriteLine(await result.Content.ReadAsStringAsync());

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> RegisterNest(LogItem<T> item)
        {
            var log = new LogModel<T>(item.User, item.OldData);
            log.History.Add(item);
            var result = await elasticsearchClient.IndexAsync(log, idx => idx.Index(collectionName));

            return result.IsValid;
        }

        public async Task<bool> RegisterNest(LogItem<T> item, object id)
        {
            var log = new LogModel<T>(item.User, item.OldData);
            log.History.Add(item);
            var result = await elasticsearchClient.IndexAsync(log, idx => idx.Index(collectionName).Id(new Id(id)));

            return result.IsValid;
        }

        public async Task<bool> RegisterOrUpdateNest(LogItem<T> item, object id, [CallerMemberName] string memberName = "", [CallerFilePath] string memberFile = "")
        {
            item.Url = httpContext.HttpContext.Request.Path.Value;
            item.Method = memberName;
            item.File = memberFile;

            var log = await GetByIdNest( id);
            if (log != null)
            {
                log.History.Add(item);
            }
            else
            {
                log = new LogModel<T>(item.User, item.OldData);
                log.History.Add(item);
            }

            var result = await elasticsearchClient.IndexAsync(log, idx => idx.Index(collectionName));

            return result.IsValid;
        }

        public async Task<LogModel<T>> GetByIdNest( object id)
        {
            try
            {
                var p = new DocumentPath<LogModel<T>>(new Id(id));

                var response = await elasticsearchClient.SearchAsync<LogModel<T>>(s =>
                                                                                    s.Size(1)
                                                                                    .Query(q =>
                                                                                        q.Match(m => m.Field(f => f.ObjectId)
                                                                                                      .Query(id.ToString()
                                                                                                      ))
                                                                                        )
                                                                                    );

                return response.Documents.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}
