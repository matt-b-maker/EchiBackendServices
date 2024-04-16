using Microsoft.EntityFrameworkCore;

namespace EchiBackendServices.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ClientModel> Clients { get; set; }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<Agency> Agencies { get; set; }
    // Other DbSets for your entities

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<ClientModel>()
    //        .HasKey(p => p.Id); // Configure Id as primary key  
    //    modelBuilder.Entity<Agency>().HasKey(p => p.Id);
    //    modelBuilder.Entity<Agent>().HasKey(p => p.Id);
    //}
}