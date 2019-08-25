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

        /// <summary>
        /// Метка права входа в систему
        /// </summary>
        public bool CanLogin { get; set; }

        /// <summary>
        /// Пароль используется только при регистрации, здесь не нужен
        /// </summary>
        public new string Password
        {
            private set { }
            get { return null; }
        }
    }
}