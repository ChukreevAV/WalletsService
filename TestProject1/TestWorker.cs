using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WalletsService.MiddleLayer;
using WalletsService.Models;

namespace TestProject1
{
    [TestClass] public class TestWorker
    {
        private IWorker _worker;
        private readonly Guid _userGuid1 = Guid.Parse("50e28c3f-4267-4c5d-9b70-e19970a0086d");
        private readonly Guid _userGuid2 = Guid.Parse("11b25905-07d4-4f02-823e-ada3070e37ba");

        [TestInitialize]
        public void Setup()
        {
            var builder = Host.CreateDefaultBuilder();

            builder.ConfigureServices((_, services) =>
            {
                services.AddDbContextFactory<WalletsContext>(opt
                    => opt.UseInMemoryDatabase("Wallets"));

                services.AddScoped<IWorker, Worker>();
            });

            var host = builder.Build();
            _worker = host.Services.GetRequiredService<IWorker>();
        }

        [TestMethod] public void TestMethod1()
        {
            var check = _worker.CheckAccount(_userGuid1);
            Assert.IsTrue(check, "Test CheckAccount (exist)");

            check = _worker.CheckAccount(Guid.NewGuid());
            Assert.IsTrue(!check, "Test CheckAccount (not exist)");

            var balance = _worker.GetBalance(_userGuid1);
            Assert.IsTrue(balance?.balance == 2000, "Test GetBalance");

            var monthResult = _worker.GetMonthResult(_userGuid1, DateTime.Now.Month);
            Assert.IsNotNull(monthResult, "Test GetMonthResult");

            _worker.AddOperation(_userGuid1, 99.33);
            balance = _worker.GetBalance(_userGuid1);
            Assert.IsTrue(balance?.balance > 2000, "Test AddOperation");
        }

        [TestMethod] public void TestMaxBalance()
        {
            try
            {
                _worker.AddOperation(_userGuid1, 100000);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Превышен максимальный баланс", ex.Message);
            }

            try
            {
                _worker.AddOperation(_userGuid2, 10000);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Превышен максимальный баланс", ex.Message);
            }
        }
    }
}