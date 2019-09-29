using LogSample.Model.Interface;
using System;

namespace LogSample.Model.Service
{
    public class UserService : IUserService
    {
        public UserModel GetUser(Guid id)
        {
            return new UserModel { DateofBirth = DateTime.Now.AddYears(-30), Email = "email@email.com", FirstName = "Teste", lastName = "", Id = id };
        }

        public void Updateuser(UserModel user)
        {
            //save
        }
    }
}
