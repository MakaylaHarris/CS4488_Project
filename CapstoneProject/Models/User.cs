/*
 * Created by Levi Delezene 
 */

using CapstoneProject.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//added namespace-alankar
namespace CapstoneProject.Models
{
    public class User
    {
        public User(int id, string FirstName, string LastName)
        {
            this.Id = id;
            this.FirstName = FirstName;
            this.LastName = LastName;
        }

        public User(string FirstName, string LastName, string EmailAddress)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.EmailAddress = EmailAddress;
        }

        public User()
        {

        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        public override string ToString()
        {
            return FullName;
        }

        public override bool Equals(object obj)
        {
            User user = (User)obj;
            if (this.FirstName == user.FirstName && this.LastName == user.LastName)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public User save() {
            OUser oUser = new OUser();
            if (oUser.Get(Id) == null) {
                oUser.Insert(this);
            } else {
                oUser.Update(this);
            }
            return this;
        }
    }
}