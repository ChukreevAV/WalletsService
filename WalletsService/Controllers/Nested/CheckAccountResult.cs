namespace WalletsService.Controllers.Nested
{
    /// <summary>Результат проверки существования кошелька</summary>
    public class CheckAccountResult
    {
        /// <summary>true = существует</summary>
        public bool isexist { get; set; }
    }
}