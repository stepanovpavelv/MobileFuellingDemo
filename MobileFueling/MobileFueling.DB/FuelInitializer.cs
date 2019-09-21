using Microsoft.AspNetCore.Identity;
using MobileFueling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileFueling.DB
{
    /// <summary>
    /// using this class because of conflicts between HasData and autoincrement in postgresql
    /// </summary>
    public static class FuelInitializer
    {
        public static void InitializePredefinedData(UserManager<ApplicationUser> userManager, FuelDbContext context)
        {
            InitializeAdminUsers(userManager, context);

            InitializeFuelTypes(context);
        }

        private static void InitializeAdminUsers(UserManager<ApplicationUser> userManager, FuelDbContext context)
        {
            if (context.AdminUsers.Any())
                return;

            const string email = "MobileFuelAdmin@mail.ru";
            const string password = "xsa3RhsJ8B";
            var adminUser = new SystemAdmin
            {
                Email = email,
                UserName = email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            Task adminDelegate() => userManager.CreateAsync(adminUser, password);
            var adminTask = Task.Factory.StartNew(adminDelegate).Unwrap();
            adminTask.Wait();
        }

        private static void InitializeFuelTypes(FuelDbContext context)
        {
            if (context.FuelTypes.Any())
                return;

            var entities = new List<FuelType>()
            {
                new FuelType { Name = "АИ-80" },
                new FuelType { Name = "АИ-92" },
                new FuelType { Name = "АИ-95" },
                new FuelType { Name = "АИ-98" },
                new FuelType { Name = "Дизель" },
                new FuelType { Name = "Газ" }
            };
            context.FuelTypes.AddRange(entities);
            context.SaveChanges();
        }
    }
}