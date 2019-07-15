using System;
using System.Collections.Generic;
using System.Text;

namespace LogSample.Model
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string lastName { get; set; }
        public DateTime? DateofBirth { get; set; }
        public string Email { get; set; }
    }
}
