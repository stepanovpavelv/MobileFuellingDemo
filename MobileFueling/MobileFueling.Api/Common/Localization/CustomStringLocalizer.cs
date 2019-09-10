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
        public const string NO_RIGHTS_TO_RECEIVE_USERLIST = nameof(NO_RIGHTS_TO_RECEIVE_USERLIST);
        public const string NO_RIGHTS_TO_RECEIVE_USER = nameof(NO_RIGHTS_TO_RECEIVE_USER);
        public const string NO_RIGHTS_TO_ADD_OR_UPDATE_USER = nameof(NO_RIGHTS_TO_ADD_OR_UPDATE_USER);
        public const string NO_RIGHTS_TO_DELETE_USER = nameof(NO_RIGHTS_TO_DELETE_USER);
        public const string USER_NOT_FOUND = nameof(USER_NOT_FOUND);
        public const string NO_RIGHTS_TO_CREATE_UPDATE_ORDER = nameof(NO_RIGHTS_TO_CREATE_UPDATE_ORDER);
        public const string ORDER_NOT_FOUND = nameof(ORDER_NOT_FOUND);
        public const string ORDERS_NOT_FOUND = nameof(ORDERS_NOT_FOUND);
        public const string NO_RIGHTS_TO_ASSIGN_DRIVER = nameof(NO_RIGHTS_TO_ASSIGN_DRIVER);
        public const string USER_CAN_NOT_ASSIGN_DRIVER_COMPLETED_ORDER = nameof(USER_CAN_NOT_ASSIGN_DRIVER_COMPLETED_ORDER);
        public const string NO_RIGHTS_TO_CANCEL_ORDER = nameof(NO_RIGHTS_TO_CANCEL_ORDER);

        public CustomStringLocalizer()
        {
            var enDict = new Dictionary<string, string>
            {
                {USER_NOT_CREATED, "User was not created" },
                {USERNAME_NOT_FOUND, "User was not found by name" },
                {USER_PASSWORD_WRONG, "User's is incorrect. Please check it and type again" },
                {USER_CAN_NOT_LOGIN, "User can not login system or has been banned" },
                {FUEL_TYPE_CAN_NOT_FIND, "Can not find fuel type by sent id" },
                {NO_RIGHTS_TO_RECEIVE_USERLIST, "User has no rights to receive user's list" },
                {NO_RIGHTS_TO_RECEIVE_USER, "User has no rights to receive userinfo" },
                {NO_RIGHTS_TO_ADD_OR_UPDATE_USER, "User has no rights to add or update userinfo" },
                {NO_RIGHTS_TO_DELETE_USER, "User has no rights to delete user" },
                {USER_NOT_FOUND, "Can not find user by this id" },
                {NO_RIGHTS_TO_CREATE_UPDATE_ORDER, "User has no rights to add or update order" },
                {ORDER_NOT_FOUND, "Can not find order by sent id" },
                {ORDERS_NOT_FOUND, "Can not find any orders by sent request" },
                {NO_RIGHTS_TO_ASSIGN_DRIVER, "User has no rights to assign driver" },
                {USER_CAN_NOT_ASSIGN_DRIVER_COMPLETED_ORDER, "User can not assign driver on completed order" },
                {NO_RIGHTS_TO_CANCEL_ORDER, "User has no rights to cancel order" }
            };
            var ruDict = new Dictionary<string, string>
            {
                {USER_NOT_CREATED, "Пользователь не был создан" },
                {USERNAME_NOT_FOUND, "Пользователя с таким именем не существует" },
                {USER_PASSWORD_WRONG, "Пароль введён некорректно, попробуйте заново" },
                {USER_CAN_NOT_LOGIN, "Пользователь не имеет права входа в систему или заблокирован" },
                {FUEL_TYPE_CAN_NOT_FIND, "Невозможно найти тип топлива по данному идентификатору" },
                {NO_RIGHTS_TO_RECEIVE_USERLIST, "Нет прав на просмотр списка пользователей" },
                {NO_RIGHTS_TO_RECEIVE_USERLIST, "Нет прав на просмотр информации по пользователю" },
                {NO_RIGHTS_TO_ADD_OR_UPDATE_USER, "Нет прав на добавление или изменение информации по пользователю" },
                {NO_RIGHTS_TO_DELETE_USER, "Нет прав на удаление пользователя" },
                {USER_NOT_FOUND, "Невозможно найти пользователя по данному идентификатору" },
                {NO_RIGHTS_TO_CREATE_UPDATE_ORDER, "Нет прав на добавление/редактирование информации по заказу" },
                {ORDER_NOT_FOUND, "Невозможно найти заказ по данному идентификатору" },
                {ORDERS_NOT_FOUND, "Невозможно найти заказы по данному запросу" },
                {NO_RIGHTS_TO_ASSIGN_DRIVER, "Нет прав на назначение водителя" },
                {USER_CAN_NOT_ASSIGN_DRIVER_COMPLETED_ORDER, "Невозможно назначить водителя на уже завершенный заказ" },
                {NO_RIGHTS_TO_CANCEL_ORDER, "Нет прав на отмену заказа" }
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