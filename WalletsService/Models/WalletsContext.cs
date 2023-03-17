using Microsoft.EntityFrameworkCore;

namespace WalletsService.Models
{
    /// <inheritdoc/>
    public class WalletsContext : DbContext
    {
        /// <summary>Конструктор</summary>
        /// <param name="options"></param>
        public WalletsContext(DbContextOptions<WalletsContext> options) : base(options) { }

        /// <summary>Кошельки</summary>
        public DbSet<Wallet> Wallets { get; set; } = null!;

        /// <summary>Операции</summary>
        public DbSet<Operation> Operations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wallet>()
                .HasMany(w => w.Operations);
        }
    }
}