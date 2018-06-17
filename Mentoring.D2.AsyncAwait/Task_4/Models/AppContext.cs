using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Task_4.Models
{
    public class AppContext : DbContext
    {
        public AppContext() : base("App")
        {
            Database.SetInitializer<AppContext>(new AppDbInitializer());
        }
        public DbSet<UserEntity> Users { get; set; }
    }
}