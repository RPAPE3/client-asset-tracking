using Microsoft.EntityFrameworkCore;
using FintechApi.Models;

namespace FintechApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<StockAsset> StockAssets { get; set; }
        public DbSet<CryptoAsset> CryptoAssets { get; set; }
        public DbSet<CashAsset> CashAssets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure TPH inheritance for Asset
            modelBuilder.Entity<Asset>()
                .HasDiscriminator<string>("AssetType")
                .HasValue<StockAsset>("Stock")
                .HasValue<CryptoAsset>("Crypto")
                .HasValue<CashAsset>("Cash");

            // User-Asset relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.Assets)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);
        }
    }
} 