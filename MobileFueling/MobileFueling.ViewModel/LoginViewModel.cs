using System.ComponentModel.DataAnnotations;

namespace MobileFueling.ViewModel
{
    /// <summary>
    /// Модель входа в систему
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}