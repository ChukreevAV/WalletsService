using WalletsService.MiddleLayer.Nested;

namespace WalletsService.MiddleLayer
{
    /// <summary>Интерфейс бизнес логики</summary>
    public interface IWorker
    {
        /// <summary>Проверяем наличие кошелька</summary>
        /// <param name="userGuid">Код пользователя</param>
        /// <returns></returns>
        bool CheckAccount(Guid userGuid);

        /// <summary>Добавляем транзакцию</summary>
        /// <param name="userGuid">Код пользователя</param>
        /// <param name="value">Деньги</param>
        void AddOperation(Guid userGuid, double value);

        /// <summary>Отчёт за месяц</summary>
        /// <param name="userGuid">Код пользователя</param>
        /// <param name="month">Месяц</param>
        /// <returns></returns>
        MonthResult? GetMonthResult(Guid userGuid, int month);

        /// <summary>Получить баланс</summary>
        /// <param name="userGuid">Код пользователя</param>
        /// <returns></returns>
        BalanceResult? GetBalance(Guid userGuid);
    }
}