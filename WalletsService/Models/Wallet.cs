namespace WalletsService.Models
{
    /// <summary>Кошелек</summary>
    public class Wallet
    {
        private const double MaxBalanceIdentified = 100000;

        private const double MaxBalanceUnidentified = 10000;

        public int Id { get; set; }

        /// <summary>Guid владельца</summary>
        public Guid UserGuid { get; set; }

        /// <summary>Признак идентифицируемости</summary>
        public bool Identified { get; set; } = false;

        /// <summary>Операции</summary>
        public List<Operation> Operations { get; set; } = new();

        private void AddOperation(Operation operation)
        {
            var balance = GetBalance();
            var max = Identified ? MaxBalanceIdentified : MaxBalanceUnidentified;
            if (balance + operation.Value < max) Operations.Add(operation);
            else throw new ArgumentException("Превышен максимальный баланс");
        }

        /// <summary>Добавить операцию</summary>
        /// <param name="value">Изменение</param>
        public void AddOperation(double value)
        {
            var op = new Operation { DateTime = DateTime.Now, Value = value };
            AddOperation(op);
        }

        /// <summary>Баланс</summary>
        /// <returns></returns>
        public double GetBalance() => Operations.Sum(o => o.Value);

        /// <summary>Количество операций за месяц</summary>
        /// <param name="month">Месяц</param>
        /// <returns></returns>
        public int GetCountByMonth(int month)
            => Operations?.Where(o => o.DateTime.Month == month).Count() ?? 0;

        /// <summary>Изменение баланса за месяц</summary>
        /// <param name="month">Месяц</param>
        /// <returns></returns>
        public double GetRechargeByMonth(int month) => Operations?
            .Where(o => o.DateTime.Month == month)
            .Sum(o => o.Value) ?? 0;
    }
}