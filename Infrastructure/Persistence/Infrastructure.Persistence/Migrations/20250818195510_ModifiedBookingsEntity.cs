using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedBookingsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingLineItems_Bookings_BookingId",
                table: "BookingLineItems");

            migrationBuilder.DropIndex(
                name: "IX_BookingLineItems_BookingId",
                table: "BookingLineItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BookingLineItems_BookingId",
                table: "BookingLineItems",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingLineItems_Bookings_BookingId",
                table: "BookingLineItems",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
