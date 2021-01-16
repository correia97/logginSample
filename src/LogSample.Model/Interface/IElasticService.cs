using LogSample.Model;
using System;
using System.Threading.Tasks;

namespace LogSample.Model.Interface
{
    public interface IElasticService
    {
        Task<bool> RegisterOrUpdate<T>(LogItem<T> item, object id, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string memberFile = "") where T : class, ICloneable;
        Task<LogModel<T>> GetById<T>(object id) where T : class, ICloneable;
        Task<bool> RegisterNest<T>(LogItem<T> item) where T : class, ICloneable;
        Task<bool> RegisterNest<T>(LogItem<T> item, object id) where T : class, ICloneable;
        Task<bool> RegisterOrUpdateNest<T>(LogItem<T> item, object id, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string memberFile = "") where T : class, ICloneable;
        Task<LogModel<T>> GetByIdNest<T>(object id) where T : class, ICloneable;
    }
}
