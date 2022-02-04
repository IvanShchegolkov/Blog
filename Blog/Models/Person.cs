using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Blog.Models
{
    public class Person
    {
        public int Id { get; set; }

        public int? RoleId { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class User : IdentityUser
    {
        //public int? RoleId { get; set; }

        //public string Login { get; set; }
        //public string Password { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public override string Email { get; set; }
    }
}
