using LogSample.Model;
using System;
using System.Threading.Tasks;

namespace LogSample.Model.Interface
{
    public interface IElasticService<T> where T : class, ICloneable
    {
        Task<bool> Register(LogItem<T> item);
        Task<bool> Register(LogItem<T> item, object id);
        Task<bool> RegisterOrUpdate(LogItem<T> item, object id, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string memberFile = "");
        Task<LogModel<T>> GetById(object id);



        Task<bool> RegisterNest(LogItem<T> item);
        Task<bool> RegisterNest(LogItem<T> item, object id);
        Task<bool> RegisterOrUpdateNest(LogItem<T> item, object id, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string memberFile = "");
        Task<LogModel<T>> GetByIdNest(object id);
    }
}
