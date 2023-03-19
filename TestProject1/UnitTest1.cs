using Microsoft.EntityFrameworkCore;

using WalletsService.Controllers.Nested;
using WalletsService.Models;

namespace TestProject1
{
    [TestClass] public class UnitTest1
    {
        [TestMethod] public void TestWalletsContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<WalletsContext>();
            optionsBuilder.UseInMemoryDatabase("Wallets");
            using var context = new WalletsContext(optionsBuilder.Options);
            var count1 = context.Wallets.Count();
            Assert.IsTrue(count1 == 0);
        }

        [TestMethod] public void TestMetod1()
        {
            var str1 = InputMessageError.UserId.GetDescription();
        }
    }
}