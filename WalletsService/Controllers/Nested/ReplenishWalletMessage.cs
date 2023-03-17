namespace WalletsService.Controllers.Nested
{
    /// <summary>Сообщение для изменения баланса</summary>
    public class ReplenishWalletMessage
    {
        /// <summary>Деньги</summary>
        public double value { get; set; }
    }
}