using System;

namespace LogSample.Model.Interface
{
    public interface IUserService
    {
        UserModel GetUser(Guid id);
        void Updateuser(UserModel user);
    }
}
