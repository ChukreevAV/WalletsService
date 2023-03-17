using Microsoft.EntityFrameworkCore;
using WalletsService.MiddleLayer.Nested;
using WalletsService.Models;

namespace WalletsService.MiddleLayer
{
    /// <summary>Реализация бизнес логики</summary>
    public class Worker : IWorker
    {
        private readonly IDbContextFactory<WalletsContext> _factory;

        public Worker(IDbContextFactory<WalletsContext> factory)
        {
            _factory = factory;
            // Заполняем БД
            using var context = _factory.CreateDbContext(); 
            DataGenerator.Initialize(context);
        }

        private bool _checkAccount(WalletsContext context, Guid userGuid)
        {
            return context.Wallets.Any(w => w.UserGuid == userGuid);
        }

        private Wallet? _getWallet(WalletsContext context, Guid userGuid)
        {
            return _checkAccount(context, userGuid)
                ? context.Wallets
                    .Include(w => w.Operations)
                    .First(w => w.UserGuid == userGuid)
                : null;
        }

        /// <inheritdoc/>
        public bool CheckAccount(Guid userGuid)
        {
            using var context = _factory.CreateDbContext();
            return _checkAccount(context, userGuid);
        }

        /// <inheritdoc/>
        public void AddOperation(Guid userGuid, double value)
        {
            using var context = _factory.CreateDbContext();
            var wallet = _getWallet(context, userGuid);
            wallet?.AddOperation(value);
            context.SaveChanges();
        }

        /// <inheritdoc/>
        public MonthResult? GetMonthResult(Guid userGuid, int month)
        {
            using var context = _factory.CreateDbContext();
            var wallet = _getWallet(context, userGuid);

            if (wallet == null) return null;
            var ops = wallet.Operations
                .Where(o => o.DateTime.Month == month)
                .ToList();

            var result = new MonthResult();

            if (ops.Any())
            {
                result.count = ops.Count;
                result.recharge = ops.Sum(o => o.Value);
            }

            return result;
        }

        /// <inheritdoc/>
        public BalanceResult? GetBalance(Guid userGuid)
        {
            using var context = _factory.CreateDbContext();
            var wallet = _getWallet(context, userGuid);
            return wallet == null ? null : new BalanceResult { balance = wallet.GetBalance() };
        }
    }
}