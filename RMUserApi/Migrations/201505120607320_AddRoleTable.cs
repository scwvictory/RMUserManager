namespace RMUserApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LdapUserRoleMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LdapId = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LdapUserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LdapUserRoleLdapUserRoleMembers",
                c => new
                    {
                        LdapUserRole_Id = c.Int(nullable: false),
                        LdapUserRoleMember_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LdapUserRole_Id, t.LdapUserRoleMember_Id })
                .ForeignKey("dbo.LdapUserRoles", t => t.LdapUserRole_Id, cascadeDelete: true)
                .ForeignKey("dbo.LdapUserRoleMembers", t => t.LdapUserRoleMember_Id, cascadeDelete: true)
                .Index(t => t.LdapUserRole_Id)
                .Index(t => t.LdapUserRoleMember_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LdapUserRoleLdapUserRoleMembers", "LdapUserRoleMember_Id", "dbo.LdapUserRoleMembers");
            DropForeignKey("dbo.LdapUserRoleLdapUserRoleMembers", "LdapUserRole_Id", "dbo.LdapUserRoles");
            DropIndex("dbo.LdapUserRoleLdapUserRoleMembers", new[] { "LdapUserRoleMember_Id" });
            DropIndex("dbo.LdapUserRoleLdapUserRoleMembers", new[] { "LdapUserRole_Id" });
            DropTable("dbo.LdapUserRoleLdapUserRoleMembers");
            DropTable("dbo.LdapUserRoles");
            DropTable("dbo.LdapUserRoleMembers");
        }
    }
}
