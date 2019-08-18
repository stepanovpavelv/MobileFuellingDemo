using System;
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

        public DateTime? DateOfBirth { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }
    }
}