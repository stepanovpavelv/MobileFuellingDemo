using System;
using System.ComponentModel.DataAnnotations;

namespace MobileFueling.ViewModel
{
    /// <summary>
    /// Модель пользователя системы MobileFuelling
    /// </summary>
    public class ApplicationUserVM : BaseUserVM
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public long Id { get; set; }
    }
}