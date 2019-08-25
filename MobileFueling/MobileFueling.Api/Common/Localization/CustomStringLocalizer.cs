using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace MobileFueling.Api.Common.Localization
{
    public class CustomStringLocalizer : IStringLocalizer
    {
        private Dictionary<string, Dictionary<string, string>> _resources;

        // ключи ресурсов
        public const string USER_NOT_CREATED = nameof(USER_NOT_CREATED);
        public const string USERNAME_NOT_FOUND = nameof(USERNAME_NOT_FOUND);
        public const string USER_PASSWORD_WRONG = nameof(USER_PASSWORD_WRONG);
        public const string USER_CAN_NOT_LOGIN = nameof(USER_CAN_NOT_LOGIN);
        public const string FUEL_TYPE_CAN_NOT_FIND = nameof(FUEL_TYPE_CAN_NOT_FIND);
        public const string NO_RIGTHS_TO_RECEIVE_USERLIST = nameof(NO_RIGTHS_TO_RECEIVE_USERLIST);

        public CustomStringLocalizer()
        {
            var enDict = new Dictionary<string, string>
            {
                {USER_NOT_CREATED, "User was not created" },
                {USERNAME_NOT_FOUND, "User was not found by name" },
                {USER_PASSWORD_WRONG, "User's is incorrect. Please check it and type again" },
                {USER_CAN_NOT_LOGIN, "User can not login system or has been banned" },
                {FUEL_TYPE_CAN_NOT_FIND, "Can not find fuel type by sent id" },
                {NO_RIGTHS_TO_RECEIVE_USERLIST, "User has not rights to receive user's list" }
            };
            var ruDict = new Dictionary<string, string>
            {
                {USER_NOT_CREATED, "Пользователь не был создан" },
                {USERNAME_NOT_FOUND, "Пользователя с таким именем не существует" },
                {USER_PASSWORD_WRONG, "Пароль введён некорректно, попробуйте заново" },
                {USER_CAN_NOT_LOGIN, "Пользователь не имеет права входа в систему или заблокирован" },
                {FUEL_TYPE_CAN_NOT_FIND, "Невозможно найти тип топлива по переданному идентификатору" },
                {NO_RIGTHS_TO_RECEIVE_USERLIST, "Нет прав на просмотр списка пользователей" }
            };

            _resources = new Dictionary<string, Dictionary<string, string>>
            {
                {"en", enDict },
                {"ru", ruDict }
            };
        }

        public LocalizedString this[string name]
        {
            get
            {
                var currentCulture = CultureInfo.CurrentUICulture;
                string val = string.Empty;
                if (_resources.ContainsKey(currentCulture.Name) && _resources[currentCulture.Name].ContainsKey(name))
                {
                    val = _resources[currentCulture.Name][name];
                }
                return new LocalizedString(name, val);
            }
        }

        public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this;
        }
    }
}