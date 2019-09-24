using Microsoft.EntityFrameworkCore;
using MobileFueling.DB;
using MobileFueling.Model;
using MobileFueling.Model.Enums;
using MobileFueling.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MobileFueling.Api.ApiModels
{
    public class ApplicationUserFactory
    {
        public static ApplicationUser CreateApplicationUser(RegisterViewModel viewModel)
        {
            ApplicationUser user = null;
            var userType = (UserType)viewModel.UserType;

            switch (userType)
            {
                case UserType.Admin:
                    user = CreateDefaultApplicationUser<SystemAdmin>(viewModel);
                    break;
                case UserType.Client:
                    user = CreateDefaultApplicationUser<Client>(viewModel);
                    user.PhoneNumber = viewModel.PhoneNumber;
                    break;
                case UserType.Driver:
                    user = CreateDefaultApplicationUser<Driver>(viewModel);
                    break;
                case UserType.Manager:
                    user = CreateDefaultApplicationUser<Manager>(viewModel);
                    break;
            }

            return user;
        }

        public async static Task<ApplicationUser> GetApplicationUserAsync(FuelDbContext context, UserTypeVM userTypeVM, long id)
        {
            switch (userTypeVM)
            {
                case UserTypeVM.Admin:
                    return await context.AdminUsers.FirstOrDefaultAsync(x => x.Id == id);
                case UserTypeVM.Client:
                    return await context.ClientUsers.FirstOrDefaultAsync(x => x.Id == id);
                case UserTypeVM.Driver:
                    return await context.DriverUsers.FirstOrDefaultAsync(x => x.Id == id);
                case UserTypeVM.Manager:
                    return await context.ManagerUsers.FirstOrDefaultAsync(x => x.Id == id);
            }

            return null;
        }

        private static ApplicationUser CreateDefaultApplicationUser<T>(RegisterViewModel viewModel) where T: ApplicationUser, new()
        {
            return new T
            {
                Email = viewModel.Email,
                UserName = viewModel.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
        }
    }
}