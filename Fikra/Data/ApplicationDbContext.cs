using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SparkLink.Models.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection.Emit;
using SparkLink.Helper;
using Fikra.Models;
namespace SparkLink.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        private readonly IEncryptionProvider _encryptionProvider;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base( options) {
            _encryptionProvider = new GenerateEncryptionProvider("138d439a81694791af2e3c9bb48657d8");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
            .Property(b => b.Code)
            .HasConversion(new EncryptedStringConverter(_encryptionProvider));
            builder.Entity<Contract>()
           .HasOne(c => c.IdeaOwner)
           .WithMany() 
           .HasForeignKey(c => c.IdeaOwnerId)
           .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<Contract>()
                .HasOne(c => c.Investor)
                .WithMany() 
                .HasForeignKey(c => c.InvestorId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Request>()
        .HasOne(c => c.Investor)
        .WithMany()
        .HasForeignKey(c => c.InvestorId)
        .OnDelete(DeleteBehavior.Restrict);
                builder.Entity<Request>()
        .HasOne(c => c.IdeaOwner)
        .WithMany()
        .HasForeignKey(c => c.IdeaOwnerId)
        .OnDelete(DeleteBehavior.Restrict);



            builder.Entity<Transaction>()
           .HasOne(c => c.IdeaOwner)
           .WithMany()
           .HasForeignKey(c => c.IdeaOwnerId)
           .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
                .HasOne(c => c.Investor)
                .WithMany()
                .HasForeignKey(c => c.InvestorId)
                .OnDelete(DeleteBehavior.Restrict);



            base.OnModelCreating(builder);
        }
        public DbSet<Signature> Signatures { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Idea> Ideas { get; set; }
        public DbSet<IdeaRating>IdeaRatings { get; set; }
        public DbSet<Message>Messages { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<StripeCustomer> StripeCustomers { get; set; }
        public DbSet<StripeAccount>StripeAccounts { get; set; }

    }
}
