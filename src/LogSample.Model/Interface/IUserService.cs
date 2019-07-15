using System;
using System.Collections.Generic;
using System.Text;

namespace LogSample.Model.Interface
{
   public interface IUserService
    {
        UserModel GetUser(Guid id);
        void Updateuser(UserModel user);
    }
}
