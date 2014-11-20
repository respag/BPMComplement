using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.EntityClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Metadata.Edm;

namespace Ultimus.ComponentManager.Models
{
    public class ComponentManagerContext : DbContext
    {
        public ComponentManagerContext() : base("Name=ContextMigration") { }

        public ComponentManagerContext(string connectionString) : base(connectionString){ }

        public ComponentManagerContext(string connectionString, Boolean ProxyCreationEnabled) : base(connectionString)
        {
            base.Configuration.ProxyCreationEnabled = ProxyCreationEnabled;
        }

        public DbSet<CatConnectionTypes> ConnectionTypes { get; set; }
        public DbSet<CatConnections> CatConnections { get; set; }
        public DbSet<AuditColumnsDefinition> AuditColumnsDefinition { get; set; }
        public DbSet<AuditTablesDefinition> AuditTablesDefinition { get; set; }
        public DbSet<AuditConnectionProcess> AuditConnectionProcess { get; set; }

        public DbSet<HistoryConfiguration> HistoryConfigurations { get; set; }

        public DbSet<CatProcesses> CatProcesses { get; set; }
        public DbSet<CatSteps> CatSteps { get; set; }
        public DbSet<FormsProcess> FormsProcesses { get; set; }
        public DbSet<CatForms> CatForms { get; set; }

        public DbSet<HelpProcess> HelpProcess { get; set; }

        public DbSet<CatAuditTablesStatus> CatAuditTablesStatus { get; set; }
        public DbSet<AuditTablesVsSteps> AuditTablesVsSteps { get; set; }

        public DbSet<CatWebAplications> CatWebAplications { get; set; }
        public DbSet<WebAplicationDetails> WebAplicationDetails { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            this.Configuration.LazyLoadingEnabled = false;

            #region Connections

            // Composite primary key for CatConnectionTypes
            modelBuilder.Entity<CatConnectionTypes>().HasKey(d => new { d.IdConnectionType });

            // Composite foreign key 
            modelBuilder.Entity<CatConnections>()
                .HasRequired(c => c.CatConnectionType)
                .WithMany(d => d.Connections)
                .HasForeignKey(d => new { d.IdConnectionType });

            // Composite primary key 
            modelBuilder.Entity<CatConnections>().HasKey(d => new { d.IdConnections });

            #endregion

            #region AuditTablesConfiguration

            // Composite foreign key IdStep for AuditTablesConfiguration 
            //modelBuilder.Entity<AuditTablesConfiguration>()
            //    .HasRequired(c => c.CatSteps)
            //    .WithMany(d => d.AuditTablesConfiguration)
            //    .HasForeignKey(d => new { d.IdStep });

            // Composite foreign key IdTablesStatus for AuditTablesConfiguration 
            modelBuilder.Entity<AuditTablesDefinition>()
                .HasRequired(c => c.CatAuditTablesStatus)
                .WithMany(d => d.AuditTablesConfiguration)
                .HasForeignKey(d => new { d.IdTablesStatus });

            //Composite primary key IdTableDefinition
            modelBuilder.Entity<AuditTablesDefinition>().HasKey(d => new { d.IdTableDefinition });

            //Ignores in IdProcess
            modelBuilder.Entity<AuditTablesDefinition>().Ignore(c => c.IdProcess);

            //Key for AuditTablesStatus
            modelBuilder.Entity<CatAuditTablesStatus>().HasKey(c => c.IdTablesStatus);


            #endregion

            #region AuditColumnsDefinition

            //Composite forein key IdTableDefinition
            modelBuilder.Entity<AuditColumnsDefinition>()
                .HasRequired(c => c.TableDefinition)
                .WithMany(d => d.ColumnsDefinitions)
                .HasForeignKey(d => new { d.IdTableDefinition });

            // Composite primary key 
            modelBuilder.Entity<AuditColumnsDefinition>().HasKey(d => new { d.IdTableDefinition, d.ColumnName });

            #endregion

            #region AuditTablesVsSteps

            //// Composite primary key for CatConnectionTypes
            modelBuilder.Entity<AuditTablesVsSteps>().HasKey(d => new { d.IdTableDefinition, d.IdStep });

            // Composite foreign key 
            modelBuilder.Entity<AuditTablesVsSteps>()
                .HasRequired(c => c.CatSteps)
                .WithMany(d => d.AuditTablesVsSteps)
                .HasForeignKey(d => new { d.IdStep });

            // Composite foreign key 
            modelBuilder.Entity<AuditTablesVsSteps>()
                .HasRequired(c => c.AuditTablesDefinition)
                .WithMany(d => d.AuditTablesVsSteps)
                .HasForeignKey(d => new { d.IdTableDefinition });

            #endregion

            // Composite primary key for History Configuration
            modelBuilder.Entity<HistoryConfiguration>().HasKey(d => new { d.IdConfiguration });

            #region Form Configuration

            #region Process

            // Composite primary key for CatProcesses
            modelBuilder.Entity<CatProcesses>().HasKey(d => new { d.IdProcess });

            // Composite extention key for CatProcesses
            modelBuilder.Entity<CatProcesses>().HasRequired(t => t.AuditConnectionProcess).WithRequiredPrincipal();

            // Composite primary key for AuditConnectionProcess
            modelBuilder.Entity<AuditConnectionProcess>().HasKey(d => new { d.IdProcess });

            // Composite foreign key  ConnectionOrigin
            modelBuilder.Entity<AuditConnectionProcess>()
                .HasRequired(c => c.ConnectionOrigin)
                .WithMany(d => d.AuditConnectionOrigin)
                .HasForeignKey(d => new { d.IdConnectionOrigin }).WillCascadeOnDelete(false);

            // Composite foreign key ConnectionDestination
            modelBuilder.Entity<AuditConnectionProcess>()
                .HasRequired(c => c.ConnectionDestination)
                .WithMany(d => d.AuditConnectionDestination)
                .HasForeignKey(d => new { d.IdConnectionDestination }).WillCascadeOnDelete(false);


            // Composite foreign key ConnectionDestination
            modelBuilder.Entity<HistoryConfiguration>()
                .HasRequired(c => c.CatProcesses)
                .WithMany(d => d.HistoryConfiguration)
                .HasForeignKey(d => new { d.IdProcess }).WillCascadeOnDelete(false);

            #endregion

            #region Catalog Steps

            // Composite primary key CatSteps
            modelBuilder.Entity<CatSteps>().HasKey(d => new { d.IdStep });

            // Composite foreign key CatSteps
            modelBuilder.Entity<CatSteps>()
                .HasRequired(c => c.CatProcesses)
                .WithMany(d => d.CatSteps)
                .HasForeignKey(d => new { d.IdProcess }).WillCascadeOnDelete(false);

            #endregion

            #region Catalog Forms

            // Composite primary key CatForms
            modelBuilder.Entity<CatForms>().HasKey(d => new { d.IdForm });

            // Composite foreign key CatProcesses
            modelBuilder.Entity<CatForms>()
                .HasRequired(c => c.CatWebAplications)
                .WithMany(d => d.CatForms)
                .HasForeignKey(d => new { d.IdWebAplication }).WillCascadeOnDelete(false);

            //Ignores in FormsProcess
            modelBuilder.Entity<CatForms>().Ignore(c => c.FormFileLimit);

            //Ignores in FormsProcess
            modelBuilder.Entity<CatForms>().Ignore(c => c.FormFileLength);

            #endregion

            #region Forms  vs Process

            // Composite primary key FormsProcess
            modelBuilder.Entity<FormsProcess>().HasKey(d => new { d.IdFormProcess });

            // Composite foreign key FormsProcess
            modelBuilder.Entity<FormsProcess>()
                .HasRequired(c => c.CatSteps)
                .WithMany(d => d.FormsProcess)
                .HasForeignKey(d => new { d.IdStep }).WillCascadeOnDelete(false);

            // Composite foreign key FormsProcess
            modelBuilder.Entity<FormsProcess>()
                .HasRequired(c => c.CatForms)
                .WithMany(d => d.FormsProcess)
                .HasForeignKey(d => new { d.IdForm }).WillCascadeOnDelete(false);

            //Ignores in FormsProcess
            modelBuilder.Entity<FormsProcess>().Ignore(c => c.IdProcess);

            #endregion

            #region Help Process

            //Ignores in HelpProcess
            modelBuilder.Entity<HelpProcess>()
    .HasKey(c => c.IdHelpProcess);

            // Composite foreign key HelpProcess
            modelBuilder.Entity<HelpProcess>()
                .HasRequired(c => c.CatForms)
                .WithMany(d => d.HelpProcess)
                .HasForeignKey(d => new { d.IdForm });

            #endregion

            #endregion


            // Composite primary key for CatConnectionTypes
            modelBuilder.Entity<CatWebAplications>().HasKey(d => new { d.IdWebAplication });

            // Composite primary key for CatConnectionTypes
            modelBuilder.Entity<WebAplicationDetails>().HasKey(d => new { d.IdProcess, d.IdWebAplication });

            // Composite foreign key 
            modelBuilder.Entity<WebAplicationDetails>()
                .HasRequired(c => c.CatProcesses)
                .WithMany(d => d.WebAplicationDetails)
                .HasForeignKey(d => new { d.IdProcess });

            // Composite foreign key 
            modelBuilder.Entity<WebAplicationDetails>()
                .HasRequired(c => c.CatWebAplications)
                .WithMany(d => d.WebAplicationDetails)
                .HasForeignKey(d => new { d.IdWebAplication });
        }
    }
}