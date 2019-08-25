using MobileFueling.Model;
using MobileFueling.Model.Enums;
using MobileFueling.ViewModel;
using System;

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