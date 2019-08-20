namespace MobileFueling.Api.ApiModels.User
{
    public static class UserConstants
    {
        // roles
        public const string AdminRole = "Admin";

        // user's claims
        public const string Name = nameof(Name); // фамилия
        public const string FirstName = nameof(FirstName); // имя
        public const string MiddleName = nameof(MiddleName); // отчество
        public const string DateOfBirth = nameof(DateOfBirth); // дата рождения
        public const string CanLogin = nameof(CanLogin); // статус пользователя (активен или заблокирован)
    }
}