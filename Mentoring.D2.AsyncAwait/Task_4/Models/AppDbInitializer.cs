using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Task_4.Models
{
    public class AppDbInitializer : DropCreateDatabaseAlways<AppContext>
    {
        protected override void Seed(AppContext context)
        {
            var newUsers = Enumerable.Range(0, 10).Select(i => new UserEntity()
            {
                Age = 10 + i,
                FirstName = $"User {i}",
                LastName = $"Userov {i}"
            });
            context.Users.AddRange(newUsers);
            base.Seed(context);
        }
    }
}