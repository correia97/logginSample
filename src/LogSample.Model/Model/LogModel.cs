using System;

namespace LogSample.Model
{
    public class LogModel<T> where T : class
    {
        public LogModel(string user)
        {
            CreateDate = DateTime.Now;
            User = user;
        }
        public DateTime CreateDate { get; private set; }
        public T OldData { get; private set; }
        public T NewData { get; private set; }
        public string User { get; private set; }

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
