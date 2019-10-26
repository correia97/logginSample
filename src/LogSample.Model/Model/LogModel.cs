using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace LogSample.Model
{
    public class LogModel<T> where T : class, ICloneable
    {
        public LogModel()
        {
            History = new List<LogItem<T>>();
        }
        public LogModel(string user, T currentData)
        {
            History = new List<LogItem<T>>();
            CreateDate = DateTime.Now;
            User = user;
            ObjectId = GetId(currentData);
        }

        public DateTime CreateDate { get; set; }
        public string User { get; set; }

        public string ObjectName => typeof(T).Name;
        public string ObjectId { get; set; }

        public List<LogItem<T>> History { get; set; }

        public void AddHistory(LogItem<T> logItem)
        {
            History.Add(logItem);
        }

        private string GetId(T obj)
        {
            var propId = typeof(T).GetProperty("Id");
            return propId.GetValue(obj).ToString();
        }
    }
}
