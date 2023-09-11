using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CleverCRM.Models.CRM;

namespace CleverCRM.Data
{
    public partial class CRMContext : DbContext
    {
        public CRMContext()
        {
        }

        public CRMContext(DbContextOptions<CRMContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CleverCRM.Models.CRM.Opportunity>()
              .HasOne(i => i.Contact)
              .WithMany(i => i.Opportunities)
              .HasForeignKey(i => i.ContactId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<CleverCRM.Models.CRM.Opportunity>()
              .HasOne(i => i.OpportunityStatus)
              .WithMany(i => i.Opportunities)
              .HasForeignKey(i => i.StatusId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<CleverCRM.Models.CRM.Task>()
              .HasOne(i => i.Opportunity)
              .WithMany(i => i.Tasks)
              .HasForeignKey(i => i.OpportunityId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<CleverCRM.Models.CRM.Task>()
              .HasOne(i => i.TaskStatus)
              .WithMany(i => i.Tasks)
              .HasForeignKey(i => i.StatusId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<CleverCRM.Models.CRM.Task>()
              .HasOne(i => i.TaskType)
              .WithMany(i => i.Tasks)
              .HasForeignKey(i => i.TypeId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<CleverCRM.Models.CRM.Opportunity>()
              .Property(p => p.CloseDate)
              .HasColumnType("datetime");

            builder.Entity<CleverCRM.Models.CRM.Task>()
              .Property(p => p.DueDate)
              .HasColumnType("datetime");
            this.OnModelBuilding(builder);
        }

        public DbSet<CleverCRM.Models.CRM.Contact> Contacts { get; set; }

        public DbSet<CleverCRM.Models.CRM.Opportunity> Opportunities { get; set; }

        public DbSet<CleverCRM.Models.CRM.OpportunityStatus> OpportunityStatuses { get; set; }

        public DbSet<CleverCRM.Models.CRM.Task> Tasks { get; set; }

        public DbSet<CleverCRM.Models.CRM.TaskStatus> TaskStatuses { get; set; }

        public DbSet<CleverCRM.Models.CRM.TaskType> TaskTypes { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    
    }
}