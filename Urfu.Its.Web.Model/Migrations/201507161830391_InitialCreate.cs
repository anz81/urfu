namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Directions",
                c => new
                    {
                        uid = c.String(nullable: false, maxLength: 128),
                        okso = c.String(maxLength: 64),
                        title = c.String(),
                        ministerialCode = c.String(),
                        ugnTitle = c.String(),
                        standard = c.String(),
                        qualifications = c.String(),
                    })
                .PrimaryKey(t => t.uid)
                .Index(t => t.okso, name: "IX_Direction_okso");
            
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        uuid = c.String(nullable: false, maxLength: 128),
                        title = c.String(),
                        shortTitle = c.String(),
                        coordinator = c.String(),
                        type = c.String(),
                        competence = c.String(),
                        testUnits = c.Int(nullable: false),
                        priority = c.Decimal(nullable: false, precision: 18, scale: 2),
                        state = c.String(),
                        approvedDate = c.DateTime(),
                        comment = c.String(),
                        file = c.String(),
                        specialities = c.String(),
                        number = c.Int(),
                    })
                .PrimaryKey(t => t.uuid);
            
            CreateTable(
                "dbo.Disciplines",
                c => new
                    {
                        uid = c.String(nullable: false, maxLength: 128),
                        title = c.String(),
                        section = c.String(),
                        testUnits = c.Decimal(nullable: false, precision: 18, scale: 2),
                        file = c.String(),
                        number = c.Int(),
                    })
                .PrimaryKey(t => t.uid);
            
            CreateTable(
                "dbo.Plans",
                c => new
                    {
                        disciplineUUID = c.String(nullable: false, maxLength: 128),
                        moduleUUID = c.String(nullable: false, maxLength: 128),
                        eduplanUUID = c.String(nullable: false, maxLength: 128),
                        eduplanNumber = c.Int(),
                        versionNumber = c.String(),
                        disciplineTitle = c.String(),
                        directionId = c.String(),
                        controls = c.String(),
                        loads = c.String(),
                        terms = c.String(),
                        allTermsExtracted = c.String(),
                    })
                .PrimaryKey(t => new { t.disciplineUUID, t.moduleUUID, t.eduplanUUID })
                .ForeignKey("dbo.Modules", t => t.moduleUUID)
                .Index(t => t.moduleUUID, name: "IX_Plan_moduleUUID");
            
            CreateTable(
                "dbo.VariantContents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        moduleId = c.String(nullable: false, maxLength: 128),
                        Selectable = c.Boolean(nullable: false),
                        Limits = c.Int(),
                        VariantGroupId = c.Int(nullable: false),
                        ContentType = c.Int(nullable: false),
                        VariantSelectionGroupId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VariantGroups", t => t.VariantGroupId, cascadeDelete: true)
                .ForeignKey("dbo.VariantSelectionGroups", t => t.VariantSelectionGroupId)
                .ForeignKey("dbo.Modules", t => t.moduleId)
                .Index(t => t.moduleId)
                .Index(t => t.VariantGroupId, name: "IX_VariantContent_VariantGroupId")
                .Index(t => t.VariantSelectionGroupId, name: "IX_VariantSelectionContent_VariantGroupId");
            
            CreateTable(
                "dbo.VariantGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupType = c.Int(nullable: false),
                        TestUnits = c.Int(nullable: false),
                        VariantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Variants", t => t.VariantId)
                .Index(t => t.VariantId, name: "IX_VariantGroup_VariantId");
            
            CreateTable(
                "dbo.Variants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        directionId = c.String(nullable: false, maxLength: 128),
                        State = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directions", t => t.directionId)
                .Index(t => t.directionId, name: "IX_Variant_directionId");
            
            CreateTable(
                "dbo.VariantSelectionGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        TestUnits = c.Int(nullable: false),
                        VariantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Variants", t => t.VariantId, cascadeDelete: true)
                .Index(t => t.VariantId, name: "IX_VariantSelectionGroup_VariantId");
            
            CreateTable(
                "dbo.UserDirections",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 128),
                        DirectionId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserName, t.DirectionId })
                .ForeignKey("dbo.Directions", t => t.DirectionId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserName)
                .Index(t => t.UserName)
                .Index(t => t.DirectionId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        Patronymic = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        AdName = c.String(nullable: false, maxLength: 127),
                        ShouldChangePassword = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.AdName, name: "IX_ApplicationUser_ADName")
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        ProfileId = c.String(),
                        Year = c.String(),
                        ChairId = c.String(),
                        FormativeDivisionId = c.String(),
                        FormativeDivisionParentId = c.String(),
                        ManagingDivisionId = c.String(),
                        ManagingDivisionParentId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ip = c.String(),
                        HttpUser = c.String(),
                        Message = c.String(),
                        Exception = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Date, name: "IX_Logs_Date");
            
            CreateTable(
                "dbo.Persons",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Surname = c.String(),
                        Name = c.String(),
                        PatronymicName = c.String(),
                        Phone = c.String(),
                        EMail = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PersonId = c.String(),
                        Status = c.String(),
                        GroupId = c.String(maxLength: 127),
                        PhoneHome = c.String(),
                        PhoneMobile = c.String(),
                        PhoneWork = c.String(),
                        Email = c.String(),
                        Icq = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.GroupId, name: "IX_Student_GroupId");
            
            CreateTable(
                "dbo.ModulesInDirections",
                c => new
                    {
                        DirectionId = c.String(nullable: false, maxLength: 128),
                        ModuleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.DirectionId, t.ModuleId })
                .ForeignKey("dbo.Modules", t => t.DirectionId, cascadeDelete: true)
                .ForeignKey("dbo.Directions", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.DirectionId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.ModuleDisciplineMapping",
                c => new
                    {
                        ModuleId = c.String(nullable: false, maxLength: 128),
                        DisciplineId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ModuleId, t.DisciplineId })
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.Disciplines", t => t.DisciplineId, cascadeDelete: true)
                .Index(t => t.ModuleId)
                .Index(t => t.DisciplineId);
            
            CreateTable(
                "dbo.VariantContentRequirements",
                c => new
                    {
                        RequiredForId = c.Int(nullable: false),
                        RequirementId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RequiredForId, t.RequirementId })
                .ForeignKey("dbo.VariantContents", t => t.RequiredForId)
                .ForeignKey("dbo.VariantContents", t => t.RequirementId)
                .Index(t => t.RequiredForId)
                .Index(t => t.RequirementId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.UserDirections", "UserName", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserDirections", "DirectionId", "dbo.Directions");
            DropForeignKey("dbo.VariantContentRequirements", "RequirementId", "dbo.VariantContents");
            DropForeignKey("dbo.VariantContentRequirements", "RequiredForId", "dbo.VariantContents");
            DropForeignKey("dbo.VariantContents", "moduleId", "dbo.Modules");
            DropForeignKey("dbo.VariantGroups", "VariantId", "dbo.Variants");
            DropForeignKey("dbo.VariantSelectionGroups", "VariantId", "dbo.Variants");
            DropForeignKey("dbo.VariantContents", "VariantSelectionGroupId", "dbo.VariantSelectionGroups");
            DropForeignKey("dbo.Variants", "directionId", "dbo.Directions");
            DropForeignKey("dbo.VariantContents", "VariantGroupId", "dbo.VariantGroups");
            DropForeignKey("dbo.Plans", "moduleUUID", "dbo.Modules");
            DropForeignKey("dbo.ModuleDisciplineMapping", "DisciplineId", "dbo.Disciplines");
            DropForeignKey("dbo.ModuleDisciplineMapping", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.ModulesInDirections", "ModuleId", "dbo.Directions");
            DropForeignKey("dbo.ModulesInDirections", "DirectionId", "dbo.Modules");
            DropIndex("dbo.VariantContentRequirements", new[] { "RequirementId" });
            DropIndex("dbo.VariantContentRequirements", new[] { "RequiredForId" });
            DropIndex("dbo.ModuleDisciplineMapping", new[] { "DisciplineId" });
            DropIndex("dbo.ModuleDisciplineMapping", new[] { "ModuleId" });
            DropIndex("dbo.ModulesInDirections", new[] { "ModuleId" });
            DropIndex("dbo.ModulesInDirections", new[] { "DirectionId" });
            DropIndex("dbo.Students", "IX_Student_GroupId");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Logs", "IX_Logs_Date");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", "IX_ApplicationUser_ADName");
            DropIndex("dbo.UserDirections", new[] { "DirectionId" });
            DropIndex("dbo.UserDirections", new[] { "UserName" });
            DropIndex("dbo.VariantSelectionGroups", "IX_VariantSelectionGroup_VariantId");
            DropIndex("dbo.Variants", "IX_Variant_directionId");
            DropIndex("dbo.VariantGroups", "IX_VariantGroup_VariantId");
            DropIndex("dbo.VariantContents", "IX_VariantSelectionContent_VariantGroupId");
            DropIndex("dbo.VariantContents", "IX_VariantContent_VariantGroupId");
            DropIndex("dbo.VariantContents", new[] { "moduleId" });
            DropIndex("dbo.Plans", "IX_Plan_moduleUUID");
            DropIndex("dbo.Directions", "IX_Direction_okso");
            DropTable("dbo.VariantContentRequirements");
            DropTable("dbo.ModuleDisciplineMapping");
            DropTable("dbo.ModulesInDirections");
            DropTable("dbo.Students");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Persons");
            DropTable("dbo.Logs");
            DropTable("dbo.Groups");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UserDirections");
            DropTable("dbo.VariantSelectionGroups");
            DropTable("dbo.Variants");
            DropTable("dbo.VariantGroups");
            DropTable("dbo.VariantContents");
            DropTable("dbo.Plans");
            DropTable("dbo.Disciplines");
            DropTable("dbo.Modules");
            DropTable("dbo.Directions");
        }
    }
}
