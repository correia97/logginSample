using LogSample.Model.Enum;
using LogSample.Model.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LogSample.Model
{
    public class LogItem<T> where T : class, ICloneable
    {
        protected LogItem()
        {
        }
        public LogItem(ActionType actionType, T data)
        {
            CreateDate = DateTime.Now;
            ActionType = actionType;
            if (data != null)
                OldData = (T)data.Clone();
        }

        public LogItem(ActionType actionType, IEnumerable<T> data)
        {
            CreateDate = DateTime.Now;
            ActionType = actionType;
            if (data != null)
                OldData = (T)data.Clone();
        }

        public LogItem(ActionType actionType, IList<T> data)
        {
            CreateDate = DateTime.Now;
            ActionType = actionType;
            if (data != null)
                OldData = (T)data.Clone();
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


                if ((OldData is IList || OldData is IEnumerable) && OldData.GetType().IsGenericType)
                {
                    return null;
                }

                var item = typeof(T).GetProperty("Id");
                return item.GetValue(OldData);
            }
        }
        public void SetNewData(T data)
        {
            NewData = (T)data.Clone();

        }

        public void SetOldData(T data)
        {
            OldData = (T)data.Clone();
        }

        public void SetNewData(IList<T> data)
        {
            NewData = (T)data.Clone();
        }

        public void SetOldData(IList<T> data)
        {
            OldData = (T)data.Clone();
        }

        public void SetNewData(IEnumerable<T> data)
        {
            NewData = (T)data.Clone();

        }

        public void SetOldData(IEnumerable<T> data)
        {
            OldData = (T)data.Clone();
        }
    }

}
