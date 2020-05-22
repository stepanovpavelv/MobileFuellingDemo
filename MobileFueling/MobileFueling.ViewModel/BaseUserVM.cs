using System;
using System.ComponentModel.DataAnnotations;

namespace MobileFueling.ViewModel
{
    public abstract class BaseUserVM
    {
        /// <summary>
        /// Адрес электронной почты
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Password { get; set; }

        /// <summary>
        /// Тип пользователя
        /// </summary>
        [Required]
        public UserTypeVM UserType { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        [Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }
    }
}
