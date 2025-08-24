using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlteredDateTimeToLocalDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration_End",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Duration_Start",
                table: "Bookings");

            migrationBuilder.AddColumn<LocalDateTime>(
                name: "EndDate",
                table: "Bookings",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new NodaTime.LocalDateTime(1, 1, 1, 0, 0));

            migrationBuilder.AddColumn<LocalDateTime>(
                name: "StartDate",
                table: "Bookings",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new NodaTime.LocalDateTime(1, 1, 1, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Bookings");

            migrationBuilder.AddColumn<DateTime>(
                name: "Duration_End",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Duration_Start",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
