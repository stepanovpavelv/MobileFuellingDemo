using System.ComponentModel.DataAnnotations;

namespace MobileFueling.ViewModel
{
    /// <summary>
    /// Модель регистрации
    /// </summary>
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public short UserType { get; set; }
    }
}