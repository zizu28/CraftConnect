using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyingCraftsmanEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourlyRate_Amount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "HourlyRate_Currency",
                table: "Users",
                newName: "ProfileImageUrl");

            migrationBuilder.RenameColumn(
                name: "Bio",
                table: "Users",
                newName: "Location");

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    CraftmanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => new { x.CraftmanId, x.Id });
                    table.ForeignKey(
                        name: "FK_Project_Users_CraftmanId",
                        column: x => x.CraftmanId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkEntry",
                columns: table => new
                {
                    CraftmanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Responsibilities = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkEntry", x => new { x.CraftmanId, x.Id });
                    table.ForeignKey(
                        name: "FK_WorkEntry_Users_CraftmanId",
                        column: x => x.CraftmanId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "WorkEntry");

            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "Users",
                newName: "HourlyRate_Currency");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Users",
                newName: "Bio");

            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRate_Amount",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Users",
                type: "bit",
                nullable: true);
        }
    }
}
