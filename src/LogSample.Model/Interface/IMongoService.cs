using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LogSample.Model.Interface
{
    public interface IMongoService
    {
        Task<bool> Register<T>(LogItem<T> log) where T : class, ICloneable;
        Task<bool> Register<T>(LogItem<T> log, object id) where T : class, ICloneable;
        Task<bool> RegisterOrUpdate<T>(LogItem<T> log, object id, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string memberFile = "") where T : class, ICloneable;
        Task<LogModel<T>> GetById<T>( object id) where T : class, ICloneable;


    }
}
