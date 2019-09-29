using LogSample.Model.Enum;
using Newtonsoft.Json;
using System;

namespace LogSample.Model
{
    public class LogItem<T> where T : class
    {
        protected LogItem()
        {
        }
        public LogItem(string user, ActionType actionType, T data)
        {
            CreateDate = DateTime.Now;
            User = user;
            ActionType = actionType;
            OldData = data;
        }
        public DateTime CreateDate { get; set; }
        public T OldData { get; set; }
        public T NewData { get; set; }
        public ActionType ActionType { get; set; }
        public string User { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string File { get; set; }
        [JsonIgnore]
        public string ObjectName => typeof(T).Name;
        [JsonIgnore]
        public object ObjectId
        {
            get
            {
                if (OldData == null)
                    return null;
                var item = typeof(T).GetProperty("Id");
                return item.GetValue(OldData);
            }
        }
        public void SetNewData(T data)
        {
            NewData = data;

        }

        public void SetOldData(T data)
        {
            OldData = data;
        }

    }
}
