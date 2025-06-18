using Microsoft.EntityFrameworkCore;

namespace Learning.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<BoardGame> BoardGames => base.Set<BoardGame>();

    public DbSet<Domain> Domains => base.Set<Domain>();

    public DbSet<Mechanic> Mechanics => base.Set<Mechanic>();

    public DbSet<BoardGame_Domain> GameDomainJunctions => base.Set<BoardGame_Domain>();

    public DbSet<BoardGame_Mechanic> GameMechanicJunctions => base.Set<BoardGame_Mechanic>();
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BoardGame_Domain>().HasKey(i => new {i.BoardGameId, i.DomainId});
        
        modelBuilder.Entity<BoardGame_Domain>()
            .HasOne(junc => junc.BoardGame)
            .WithMany(game => game.DomainsJunction)
            .HasForeignKey(junc => junc.BoardGameId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BoardGame_Domain>()
            .HasOne(junc => junc.Domain)
            .WithMany(dom => dom.BoardGamesJunction)
            .HasForeignKey(junc => junc.DomainId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        
        modelBuilder.Entity<BoardGame_Mechanic>().HasKey(i => new {i.BoardGameId, i.MechanicId});

        modelBuilder.Entity<BoardGame_Mechanic>()
            .HasOne(junc => junc.BoardGame)
            .WithMany(game => game.MechanicsJunction)
            .HasForeignKey(junc => junc.BoardGameId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BoardGame_Mechanic>()
            .HasOne(junc => junc.Mechanic)
            .WithMany(mech => mech.BoardGamesJunction)
            .HasForeignKey(junc => junc.MechanicId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}