namespace WalletsService.Models
{
    /// <summary>Операция</summary>
    public class Operation
    {
        /// <summary>Ключ</summary>
        public int Id { get; set; }

        /// <summary>Ключ кошелька</summary>
        public int WalletId { get; set; }

        /// <summary>Время операции</summary>
        public DateTime DateTime { get; set; }

        /// <summary>Значение</summary>
        public double Value { get; set; }
    }
}