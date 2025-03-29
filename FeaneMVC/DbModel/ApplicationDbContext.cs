using FeaneMVC.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace FinalProject.DbModel
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserData> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<UserFilter> UserFilters { get; set; }
        public DbSet<MailCheck>  MailChecks { get; set; }
        public DbSet<MailMessage> MailMessages { get; set; }
        
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFilter>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Filters)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<MailCheck>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<MailCheck>()
                .Property(m => m.Email)
                .IsRequired();

            modelBuilder.Entity<MailCheck>()
                .HasOne(m => m.User)            
                .WithMany(u => u.MailChecks)    
                .HasForeignKey(m => m.UserId)   
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<MailMessage>()
                .HasIndex(m => new { m.UniqueId, m.UserId })
                .IsUnique();
            
            modelBuilder.Entity<Notification>()
                .HasKey(n => n.Id); 

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User) 
                .WithMany(u => u.Notifications) 
                .HasForeignKey(n => n.UserId) 
                .OnDelete(DeleteBehavior.Cascade);
        }







    }
}
