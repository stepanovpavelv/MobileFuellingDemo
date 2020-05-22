using System.ComponentModel;

namespace MobileFueling.ViewModel
{
    /// <summary>
    /// Типы пользователей в системе
    /// </summary>
    public enum UserTypeVM
    {
        [Description("Администратор")]
        Admin = 0,
        [Description("Водитель")]
        Driver,
        [Description("Клиент")]
        Client,
        [Description("Менеджер")]
        Manager
    }
}