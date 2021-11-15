using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Blog.Models
{
    public class PersonContext : DbContext
    {
        public DbSet<Person> Person { get; set; }
        public DbSet<Role> Role { get; set; }
        public PersonContext(DbContextOptions<PersonContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
