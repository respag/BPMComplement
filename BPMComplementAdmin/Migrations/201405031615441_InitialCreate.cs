namespace Ultimus.AuditManager.Admin.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditColumnsDefinition",
                c => new
                    {
                        IdTableDefinition = c.Int(nullable: false),
                        ColumnName = c.String(nullable: false, maxLength: 128),
                        IsKey = c.Boolean(nullable: false),
                        wasPublished = c.Boolean(nullable: false),
                        isDisable = c.Boolean(nullable: false),
                        DataType = c.String(maxLength: 100),
                        DataLength = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdTableDefinition, t.ColumnName })
                .ForeignKey("dbo.AuditTablesDefinition", t => t.IdTableDefinition, cascadeDelete: true)
                .Index(t => t.IdTableDefinition);
            
            CreateTable(
                "dbo.AuditTablesDefinition",
                c => new
                    {
                        IdTableDefinition = c.Int(nullable: false, identity: true),
                        TableName = c.String(nullable: false, maxLength: 200),
                        IdTablesStatus = c.String(nullable: false, maxLength: 4, fixedLength: true, unicode: false),
                    })
                .PrimaryKey(t => t.IdTableDefinition)
                .ForeignKey("dbo.CatAuditTablesStatus", t => t.IdTablesStatus, cascadeDelete: true)
                .Index(t => t.IdTablesStatus);
            
            CreateTable(
                "dbo.AuditTablesVsSteps",
                c => new
                    {
                        IdTableDefinition = c.Int(nullable: false),
                        IdStep = c.Int(nullable: false),
                        isDisabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdTableDefinition, t.IdStep })
                .ForeignKey("dbo.AuditTablesDefinition", t => t.IdTableDefinition, cascadeDelete: true)
                .ForeignKey("dbo.CatSteps", t => t.IdStep, cascadeDelete: true)
                .Index(t => t.IdTableDefinition)
                .Index(t => t.IdStep);
            
            CreateTable(
                "dbo.CatSteps",
                c => new
                    {
                        IdStep = c.Int(nullable: false, identity: true),
                        StepName = c.String(nullable: false, maxLength: 200),
                        IdProcess = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdStep)
                .ForeignKey("dbo.CatProcesses", t => t.IdProcess)
                .Index(t => t.IdProcess);
            
            CreateTable(
                "dbo.CatProcesses",
                c => new
                    {
                        IdProcess = c.Int(nullable: false, identity: true),
                        ProcessName = c.String(nullable: false, maxLength: 100),
                        RootLocation = c.String(nullable: false),
                        ProcessVersion = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdProcess);
            
            CreateTable(
                "dbo.AuditConnectionProcess",
                c => new
                    {
                        IdProcess = c.Int(nullable: false),
                        IdConnectionOrigin = c.Int(nullable: false),
                        IdConnectionDestination = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdProcess)
                .ForeignKey("dbo.CatConnections", t => t.IdConnectionDestination)
                .ForeignKey("dbo.CatConnections", t => t.IdConnectionOrigin)
                .ForeignKey("dbo.CatProcesses", t => t.IdProcess)
                .Index(t => t.IdProcess)
                .Index(t => t.IdConnectionOrigin)
                .Index(t => t.IdConnectionDestination);
            
            CreateTable(
                "dbo.CatConnections",
                c => new
                    {
                        IdConnections = c.Int(nullable: false, identity: true),
                        ConnectionString = c.String(nullable: false),
                        ConnectionName = c.String(nullable: false, maxLength: 100),
                        IdConnectionType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdConnections)
                .ForeignKey("dbo.CatConnectionTypes", t => t.IdConnectionType, cascadeDelete: true)
                .Index(t => t.IdConnectionType);
            
            CreateTable(
                "dbo.CatConnectionTypes",
                c => new
                    {
                        IdConnectionType = c.Int(nullable: false),
                        ConnectionName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.IdConnectionType);
            
            CreateTable(
                "dbo.HistoryConfiguration",
                c => new
                    {
                        IdConfiguration = c.Int(nullable: false, identity: true),
                        IdProcess = c.Int(nullable: false),
                        CompletedTaskDeleted = c.Boolean(nullable: false),
                        IncidentAborted = c.Boolean(nullable: false),
                        IncidentCompleted = c.Boolean(nullable: false),
                        IncidentInitiated = c.Boolean(nullable: false),
                        StepAborted = c.Boolean(nullable: false),
                        TaskActivated = c.Boolean(nullable: false),
                        TaskAssigned = c.Boolean(nullable: false),
                        TaskCompleted = c.Boolean(nullable: false),
                        TaskConferred = c.Boolean(nullable: false),
                        TaskDelayed = c.Boolean(nullable: false),
                        TaskLate = c.Boolean(nullable: false),
                        TaskResubmitted = c.Boolean(nullable: false),
                        TaskReturned = c.Boolean(nullable: false),
                        TaskSubmitFailed = c.Boolean(nullable: false),
                        TasksPerDayThresholdReached = c.Boolean(nullable: false),
                        QueueTaskActivated = c.Boolean(nullable: false),
                        TaskDeletedOnMinResponseComplete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdConfiguration)
                .ForeignKey("dbo.CatProcesses", t => t.IdProcess)
                .Index(t => t.IdProcess);
            
            CreateTable(
                "dbo.WebAplicationDetails",
                c => new
                    {
                        IdProcess = c.Int(nullable: false),
                        IdWebAplication = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdProcess, t.IdWebAplication })
                .ForeignKey("dbo.CatProcesses", t => t.IdProcess, cascadeDelete: true)
                .ForeignKey("dbo.CatWebAplications", t => t.IdWebAplication, cascadeDelete: true)
                .Index(t => t.IdProcess)
                .Index(t => t.IdWebAplication);
            
            CreateTable(
                "dbo.CatWebAplications",
                c => new
                    {
                        IdWebAplication = c.Int(nullable: false, identity: true),
                        WebAplicationName = c.String(nullable: false, maxLength: 300),
                        WebAplicationPath = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IdWebAplication);
            
            CreateTable(
                "dbo.CatForms",
                c => new
                    {
                        IdForm = c.Int(nullable: false, identity: true),
                        FormFile = c.String(nullable: false),
                        FormLabel = c.String(nullable: false, maxLength: 200),
                        IdWebAplication = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdForm)
                .ForeignKey("dbo.CatWebAplications", t => t.IdWebAplication)
                .Index(t => t.IdWebAplication);
            
            CreateTable(
                "dbo.FormsProcess",
                c => new
                    {
                        IdFormProcess = c.Int(nullable: false, identity: true),
                        IdStep = c.Int(nullable: false),
                        FormOrder = c.Int(nullable: false),
                        IdForm = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdFormProcess)
                .ForeignKey("dbo.CatForms", t => t.IdForm)
                .ForeignKey("dbo.CatSteps", t => t.IdStep)
                .Index(t => t.IdStep)
                .Index(t => t.IdForm);
            
            CreateTable(
                "dbo.HelpProcess",
                c => new
                    {
                        IdHelpProcess = c.Int(nullable: false, identity: true),
                        IdForm = c.Int(nullable: false),
                        Control = c.String(nullable: false, maxLength: 300),
                        Text = c.String(nullable: false, unicode: false, storeType: "text"),
                        ControlLabel = c.String(nullable: false, maxLength: 300),
                    })
                .PrimaryKey(t => t.IdHelpProcess)
                .ForeignKey("dbo.CatForms", t => t.IdForm, cascadeDelete: true)
                .Index(t => t.IdForm);
            
            CreateTable(
                "dbo.CatAuditTablesStatus",
                c => new
                    {
                        IdTablesStatus = c.String(nullable: false, maxLength: 4, fixedLength: true, unicode: false),
                        StatusName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.IdTablesStatus);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuditColumnsDefinition", "IdTableDefinition", "dbo.AuditTablesDefinition");
            DropForeignKey("dbo.AuditTablesDefinition", "IdTablesStatus", "dbo.CatAuditTablesStatus");
            DropForeignKey("dbo.AuditTablesVsSteps", "IdStep", "dbo.CatSteps");
            DropForeignKey("dbo.CatSteps", "IdProcess", "dbo.CatProcesses");
            DropForeignKey("dbo.WebAplicationDetails", "IdWebAplication", "dbo.CatWebAplications");
            DropForeignKey("dbo.HelpProcess", "IdForm", "dbo.CatForms");
            DropForeignKey("dbo.FormsProcess", "IdStep", "dbo.CatSteps");
            DropForeignKey("dbo.FormsProcess", "IdForm", "dbo.CatForms");
            DropForeignKey("dbo.CatForms", "IdWebAplication", "dbo.CatWebAplications");
            DropForeignKey("dbo.WebAplicationDetails", "IdProcess", "dbo.CatProcesses");
            DropForeignKey("dbo.HistoryConfiguration", "IdProcess", "dbo.CatProcesses");
            DropForeignKey("dbo.AuditConnectionProcess", "IdProcess", "dbo.CatProcesses");
            DropForeignKey("dbo.AuditConnectionProcess", "IdConnectionOrigin", "dbo.CatConnections");
            DropForeignKey("dbo.AuditConnectionProcess", "IdConnectionDestination", "dbo.CatConnections");
            DropForeignKey("dbo.CatConnections", "IdConnectionType", "dbo.CatConnectionTypes");
            DropForeignKey("dbo.AuditTablesVsSteps", "IdTableDefinition", "dbo.AuditTablesDefinition");
            DropIndex("dbo.HelpProcess", new[] { "IdForm" });
            DropIndex("dbo.FormsProcess", new[] { "IdForm" });
            DropIndex("dbo.FormsProcess", new[] { "IdStep" });
            DropIndex("dbo.CatForms", new[] { "IdWebAplication" });
            DropIndex("dbo.WebAplicationDetails", new[] { "IdWebAplication" });
            DropIndex("dbo.WebAplicationDetails", new[] { "IdProcess" });
            DropIndex("dbo.HistoryConfiguration", new[] { "IdProcess" });
            DropIndex("dbo.CatConnections", new[] { "IdConnectionType" });
            DropIndex("dbo.AuditConnectionProcess", new[] { "IdConnectionDestination" });
            DropIndex("dbo.AuditConnectionProcess", new[] { "IdConnectionOrigin" });
            DropIndex("dbo.AuditConnectionProcess", new[] { "IdProcess" });
            DropIndex("dbo.CatSteps", new[] { "IdProcess" });
            DropIndex("dbo.AuditTablesVsSteps", new[] { "IdStep" });
            DropIndex("dbo.AuditTablesVsSteps", new[] { "IdTableDefinition" });
            DropIndex("dbo.AuditTablesDefinition", new[] { "IdTablesStatus" });
            DropIndex("dbo.AuditColumnsDefinition", new[] { "IdTableDefinition" });
            DropTable("dbo.CatAuditTablesStatus");
            DropTable("dbo.HelpProcess");
            DropTable("dbo.FormsProcess");
            DropTable("dbo.CatForms");
            DropTable("dbo.CatWebAplications");
            DropTable("dbo.WebAplicationDetails");
            DropTable("dbo.HistoryConfiguration");
            DropTable("dbo.CatConnectionTypes");
            DropTable("dbo.CatConnections");
            DropTable("dbo.AuditConnectionProcess");
            DropTable("dbo.CatProcesses");
            DropTable("dbo.CatSteps");
            DropTable("dbo.AuditTablesVsSteps");
            DropTable("dbo.AuditTablesDefinition");
            DropTable("dbo.AuditColumnsDefinition");
        }
    }
}
