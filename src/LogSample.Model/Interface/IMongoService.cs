using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LogSample.Model.Interface
{
    public interface IMongoService<T> where T : class, ICloneable
    {
        Task<bool> Register(LogItem<T> log);
        Task<bool> Register(LogItem<T> log, object id);
        Task<bool> RegisterOrUpdate(LogItem<T> log, object id, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string memberFile = "");
        Task<LogModel<T>> GetById( object id);


    }
}
