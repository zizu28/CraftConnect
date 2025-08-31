using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedBookingEntityWithOwnedJobDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Details",
                table: "Bookings",
                newName: "Details_Description");

            migrationBuilder.AddColumn<Guid>(
                name: "Details_Id",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details_Id",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "Details_Description",
                table: "Bookings",
                newName: "Details");
        }
    }
}
