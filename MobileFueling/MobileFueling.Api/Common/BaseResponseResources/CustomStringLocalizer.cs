﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace MobileFueling.Api.Common.BaseResponseResources
{
    public class CustomStringLocalizer : IStringLocalizer
    {
        private Dictionary<string, Dictionary<string, string>> _resources;

        // ключи ресурсов
        public const string USER_NOT_CREATED = nameof(USER_NOT_CREATED);
        public const string USERNAME_NOT_FOUND = nameof(USERNAME_NOT_FOUND);
        public const string USER_PASSWORD_WRONG = nameof(USER_PASSWORD_WRONG);

        public CustomStringLocalizer()
        {
            var enDict = new Dictionary<string, string>
            {
                {USER_NOT_CREATED, "User was not created" },
                {USERNAME_NOT_FOUND, "User was not found by name" },
                {USER_PASSWORD_WRONG, "User's is incorrect. Please check it and type again" }
            };
            var ruDict = new Dictionary<string, string>
            {
                {USER_NOT_CREATED, "Пользователь не был создан" },
                {USERNAME_NOT_FOUND, "Пользователя с таким именем не существует" },
                {USER_PASSWORD_WRONG, "Пароль введён некорректно, попробуйте заново" }
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