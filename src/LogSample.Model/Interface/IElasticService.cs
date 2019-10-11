using System.Threading.Tasks;

namespace LogSample.Model.Interface
{
    public interface IElasticService<T> where T : class
    {
        Task<bool> Register(LogItem<T> log);
        Task<bool> Register(LogItem<T> log, object id);
        Task<bool> RegisterOrUpdate(LogItem<T> log, object id, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",  [System.Runtime.CompilerServices.CallerFilePath] string memberFile = "");
        Task<LogModel<T>> GetById( object id);



        Task<bool> RegisterNest(LogItem<T> log);
        Task<bool> RegisterNest(LogItem<T> log, object id);
        Task<bool> RegisterOrUpdateNest(LogItem<T> log, object id, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string memberFile = "");
        Task<LogModel<T>> GetByIdNest( object id);
    }
}
