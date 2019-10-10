using System.Threading.Tasks;

namespace LogSample.Model.Interface
{
    public interface IElasticService<T> where T : class
    {
        Task<bool> Register(LogItem<T> log, string objectName);
        Task<bool> Register(LogItem<T> log, string objectName, object id);
        Task<bool> RegisterOrUpdate(LogItem<T> log, string objectName, object id, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",  [System.Runtime.CompilerServices.CallerFilePath] string memberFile = "");
        Task<LogModel<T>> GetById(string objectName, object id);



        Task<bool> RegisterNest(LogItem<T> log, string objectName);
        Task<bool> RegisterNest(LogItem<T> log, string objectName, object id);
        Task<bool> RegisterOrUpdateNest(LogItem<T> log, string objectName, object id, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string memberFile = "");
        Task<LogModel<T>> GetByIdNest(string objectName, object id);
    }
}
