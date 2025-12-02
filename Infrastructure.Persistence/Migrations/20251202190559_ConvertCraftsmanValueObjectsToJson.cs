using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConvertCraftsmanValueObjectsToJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkEntry",
                table: "WorkEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skill",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WorkEntry");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Skill");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkEntryId",
                table: "WorkEntry",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SkillId",
                table: "Skill",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkEntry",
                table: "WorkEntry",
                columns: new[] { "CraftmanId", "WorkEntryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skill",
                table: "Skill",
                columns: new[] { "CraftmanId", "SkillId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkEntry",
                table: "WorkEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skill",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "WorkEntryId",
                table: "WorkEntry");

            migrationBuilder.DropColumn(
                name: "SkillId",
                table: "Skill");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "WorkEntry",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Skill",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkEntry",
                table: "WorkEntry",
                columns: new[] { "CraftmanId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skill",
                table: "Skill",
                columns: new[] { "CraftmanId", "Id" });
        }
    }
}
