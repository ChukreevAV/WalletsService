namespace WalletsService.Models
{
    /// <summary>Заполняем БД</summary>
    public static class DataGenerator
    {
        public static Wallet GetWallet1()
        {
            var wallet = new Wallet
            {
                UserGuid = Guid.Parse("50e28c3f-4267-4c5d-9b70-e19970a0086d"),
                Identified = true
            };

            wallet.Operations.Add(new Operation
            {
                DateTime = DateTime.Now,
                Value = 1000
            });

            wallet.Operations.Add(new Operation
            {
                DateTime = DateTime.Now + new TimeSpan(1, 1, 0, 0),
                Value = 1000
            });

            return wallet;
        }

        public static Wallet GetWallet2()
        {
            var wallet = new Wallet
            {
                UserGuid = Guid.Parse("11b25905-07d4-4f02-823e-ada3070e37ba"),
                Identified = false
            };

            wallet.Operations.Add(new Operation
            {
                DateTime = DateTime.Now,
                Value = 100
            });

            wallet.Operations.Add(new Operation
            {
                DateTime = DateTime.Now + new TimeSpan(1, 1, 0, 0),
                Value = 200
            });

            return wallet;
        }

        public static void Initialize(WalletsContext context)
        {
            if (context.Wallets.Any())
            {
                return;   // Data was already seeded
            }

            context.Wallets.Add(GetWallet1());
            context.Wallets.Add(GetWallet2());

            context.SaveChanges();
        }
    }
}