using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

//fait le lien entre les classes C# et la base 
public class ParcInformatiqueContext : DbContext
{
    //table des ordinateurs
    public DbSet<Ordinateur> Ordinateurs { get; set; }
    
    // table des réparations
    public DbSet<Reparation> Reparations { get; set; }

     public DbSet<User> Users { get; set; }

    public ParcInformatiqueContext(DbContextOptions<ParcInformatiqueContext> options)
        : base(options)
    {
    }

    //connexion à la base
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        optionsBuilder.UseSqlite("Data Source=parcinfo.db"); 

    }


    //Q3. TP4
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Ordinateur>()
                .OwnsOne(o => o.Caracteristiques);

            // un ordi a pls réparation et chaque réparation concerne un ordinateur
            modelBuilder.Entity<Ordinateur>()
                .HasMany(o => o.Reparations)
                .WithOne(r => r.UnOrdinateur)
                .HasForeignKey("OrdinateurId");

            // on stocke le prix en nombre réel
            modelBuilder.Entity<Ordinateur>()
                .Property(o => o.Prix)
                .HasConversion<double>();


            //login est unique
            modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();
        }

    
}