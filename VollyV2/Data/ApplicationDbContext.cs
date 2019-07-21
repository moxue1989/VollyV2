using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VollyV2.Data.Volly;
using VollyV2.Models;
using VollyV2.Models.Volly;

namespace VollyV2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
       

        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cause> Causes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<OpportunityImage> OpportunityImages { get; set; }
        public DbSet<Occurrence> Occurrences { get; set; }
        public DbSet<ApplicationOccurrence> ApplicationsOccurrence { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<VolunteerHours> VolunteerHours { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<UserCause> UserCauses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationOccurrence>()
                .HasKey(k => new { k.ApplicationId, k.OccurrenceId });
            builder.Entity<ApplicationOccurrence>()
                .HasOne(ao => ao.Application)
                .WithMany(ao => ao.Occurrences)
                .HasForeignKey(ao => ao.ApplicationId);
            builder.Entity<ApplicationOccurrence>()
                .HasOne(ao => ao.Occurrence)
                .WithMany(ao => ao.Applications)
                .HasForeignKey(ao => ao.OccurrenceId);
            builder.Entity<Company>()
                .HasIndex(c => c.CompanyCode)
                .IsUnique();

            builder.Entity<UserCause>()
                .HasKey(u => new { u.UserId, u.CauseId });
            builder.Entity<UserCause>()
                .HasOne(uc => uc.User)
                .WithMany(uc => uc.Causes)
                .HasForeignKey(uc => uc.UserId);
            builder.Entity<UserCause>()
                .HasOne(uc => uc.Cause)
                .WithMany(uc => uc.Users)
                .HasForeignKey(uc => uc.CauseId);
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<IdentityRole> IdentityRoles { get; set; }
    }
}
