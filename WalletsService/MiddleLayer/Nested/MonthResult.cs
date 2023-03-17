namespace WalletsService.MiddleLayer.Nested
{
    /// <summary>Отчет за месяц</summary>
    public class MonthResult
    {
        /// <summary>Количество транзакций</summary>
        public int count { get; set; }

        /// <summary>Изменение</summary>
        public double recharge { get; set; }
    }
}