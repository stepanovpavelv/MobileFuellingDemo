using System.ComponentModel;

namespace MobileFueling.ViewModel
{
    /// <summary>
    /// Типы пользователей в системе
    /// </summary>
    public enum UserTypeVM
    {
        [Description("Admin")]
        Admin = 0,
        [Description("Driver")]
        Driver,
        [Description("Client")]
        Client,
        [Description("Manager")]
        Manager
    }
}