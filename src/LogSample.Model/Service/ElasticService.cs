﻿using LogSample.Model.Interface;
using LogSample.Model.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace LogSample.Model.Service
{
    public class ElasticService<T> : IElasticService<T> where T : class
    {
        private string ElasticUrl { get; set; }
        private HttpClient Client { get; set; }
        private IHttpContextAccessor httpContext { get; set; }
        private ElasticClient elasticsearchClient { get; set; }
        public ElasticService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            if (Client == null)
                Client = new HttpClient();
            ElasticUrl = config.GetSection("Elastic").Value;
            Client.BaseAddress = new Uri(ElasticUrl);
            httpContext = httpContextAccessor;


            var node = new Uri(ElasticUrl);
            var settings = new ConnectionSettings(node);
            elasticsearchClient = new ElasticClient(settings);

        }
        public async Task<LogModel<T>> GetById(string objectName, object id)
        {
            var result = await Client.GetAsync($"{objectName.ToLower()}/_doc/{id}");
            if (result.IsSuccessStatusCode)
            {
                var temp = await result.Content.ReadAsStringAsync();
                Debug.WriteLine(temp);
                var data = JsonConvert.DeserializeObject<ElasticModel<LogModel<T>>>(temp);

                return data._source;
            }
            return null;
        }

        public async Task<bool> Register(LogItem<T> log, string objectName, object id)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, $"{objectName.ToLower()}/_doc");
            message.Content = new StringContent(JsonConvert.SerializeObject(log), Encoding.UTF8, "application/json");
            var result = await Client.SendAsync(message);

            Debug.WriteLine(result.ReasonPhrase);

            var data = JsonConvert.DeserializeObject<ElasticModel<T>>(await result.Content.ReadAsStringAsync());
            Debug.WriteLine(data);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Register(LogItem<T> log, string objectName)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, $"{objectName.ToLower()}/_doc");
            message.Content = new StringContent(JsonConvert.SerializeObject(log), Encoding.UTF8, "application/json");
            var result = await Client.SendAsync(message);

            Debug.WriteLine(result.ReasonPhrase);

            Debug.WriteLine(await result.Content.ReadAsStringAsync());

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> RegisterOrUpdate(LogItem<T> log, string objectName, object id, string memberName, string memberFile)
        {
            log.Url = httpContext.HttpContext.Request.Path.Value;
            log.Method = memberName;
            log.File = memberFile;

            var item = await GetById(objectName, id);
            if (item != null)
            {
                item.History.Add(log);
            }
            else
            {
                item = new LogModel<T>(log.User, log.OldData);
                item.History.Add(log);
            }

            var message = new HttpRequestMessage(HttpMethod.Put, $"{objectName.ToLower()}/_doc/{id}");
            message.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            var result = await Client.SendAsync(message);

            Debug.WriteLine(result.ReasonPhrase);

            Debug.WriteLine(await result.Content.ReadAsStringAsync());

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> RegisterNest(LogItem<T> log, string objectName)
        {
            var result = await elasticsearchClient.IndexAsync(log, idx => idx.Index(objectName));

            return result.IsValid;
        }

        public async Task<bool> RegisterNest(LogItem<T> log, string objectName, object id)
        {
            var result = await elasticsearchClient.IndexAsync(log, idx => idx.Index(objectName).Id(new Id(id)));

            return result.IsValid;
        }

        public async Task<bool> RegisterOrUpdateNest(LogItem<T> log, string objectName, object id, [CallerMemberName] string memberName = "", [CallerFilePath] string memberFile = "")
        {
            log.Url = httpContext.HttpContext.Request.Path.Value;
            log.Method = memberName;
            log.File = memberFile;

            var item = await GetByIdNest(objectName, id);
            if (item != null)
            {
                item.History.Add(log);
            }
            else
            {
                item = new LogModel<T>(log.User, log.OldData);
                item.History.Add(log);
            }

            var result = await elasticsearchClient.IndexAsync(log, idx => idx.Index(objectName));

            return result.IsValid;
        }

        public async Task<LogModel<T>> GetByIdNest(string objectName, object id)
        {
            var p = new DocumentPath<LogModel<T>>(new Id(id));
            var response = await elasticsearchClient.GetAsync<LogModel<T>>(p, idx => idx.Index(objectName));

            return response.Source;
        }
    }
}