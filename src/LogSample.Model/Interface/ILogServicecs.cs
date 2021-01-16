using System;
using System.Threading.Tasks;

namespace LogSample.Model.Interface
{
    public interface ILogServicecs<T> where T : class, ICloneable
    {
        Task RegisterOrUpdate(LogItem<T> item, object id, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string memberFile = "");
        Task Register(LogItem<T> item, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string memberFile = "");

    }
}
