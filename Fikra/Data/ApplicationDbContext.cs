﻿using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SparkLink.Models.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection.Emit;
using SparkLink.Helper;
using Fikra.Models;
using Fikra.Controllers;
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
            builder.Entity<CV>().HasOne(c=>c.ApplicationUser).WithOne(x=>x.CV).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CV>().HasMany(x => x.Technologies).WithOne(x => x.CV).HasForeignKey(x => x.CVId).OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }
        public DbSet<Signature> Signatures { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Idea> Ideas { get; set; }
        public DbSet<IdeaRating>IdeaRatings { get; set; }
        public DbSet<Message>Messages { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<StripeCustomer> StripeCustomers { get; set; }
        public DbSet<StripeAccount>StripeAccounts { get; set; }
        public DbSet<JoinRequest> JoinRequests { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<ChatGroup>ChatGroups { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<PenaltyPoint> penaltyPoints { get; set; }
        public DbSet<MoneyTransferRequest> moneyTransferRequests { get; set; }
        public DbSet<Draft> Drafts { get; set; }
        public DbSet<GroupMessage>GroupesMessages { get; set; }


    }
}
