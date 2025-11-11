using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;

namespace AssetManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

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
        public DbSet<Division> Divisions { get; set; }
        public DbSet<User> Users { get; set; }
       // public object UserProfiles { get; internal set; }
        public DbSet<UserProfile> UserProfile { get; set; }
    }
}
