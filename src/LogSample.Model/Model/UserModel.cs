using System;

namespace LogSample.Model
{
    public class UserModel : ICloneable
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string lastName { get; set; }
        public DateTime? DateofBirth { get; set; }
        public string Email { get; set; }


        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
