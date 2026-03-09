using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;

namespace AssetManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<Status> AssetStatuses { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<AssetLocation> AssetLocations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Barcode> Barcode { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<AssetTransferLog> AssetTransferLogs { get; set; }
        public DbSet<ITAssetDetail> ITAssetDetails { get; set; }
        public DbSet<Printer> Printers { get; set; }
        public DbSet<UserProfile> UserProfile { get; internal set; }
        public DbSet<PrinterType> PrinterTypes { get; set; }  
        public DbSet<Cctv> Cctv { get; set; }
        public DbSet<Consumable> Consumables { get; set; }
        public DbSet<ConsumableStock> ConsumableStocks { get; set; }
        public DbSet<ConsumableTransaction> ConsumableTransactions { get; set; }
        public DbSet<Accessory> Accessories { get; set; }   
        public DbSet<Category> Categories { get; set; }   
        public DbSet<Component> Components { get; set; }
        public DbSet<Tv> Tv { get; set; }
        public DbSet<Telephone> Telephone { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* One-to-One: Consumable → Stock */
            modelBuilder.Entity<Consumable>()
                .HasOne(c => c.Stock)
                .WithOne(s => s.Consumable)
                .HasForeignKey<ConsumableStock>(s => s.ConsumableId);

            /* Decimal precision */
            modelBuilder.Entity<ConsumableStock>()
                .Property(p => p.AvailableQuantity)
                .HasPrecision(10, 2);

            modelBuilder.Entity<ConsumableTransaction>()
                .Property(p => p.Quantity)
                .HasPrecision(10, 2);
        }

        // Override the OnModelCreating method to configure the model
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // Explicitly configure the primary key for Status
        //    modelBuilder.Entity<Status>()
        //        .HasKey(s => s.StatusId); // Specify the primary key explicitly

        //    // You can add more model configurations here if needed.
        //}
    }
}
