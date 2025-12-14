using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedCustomerEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_Location_Latitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Address_Location_Longitude",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Address_Location_Latitude",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Address_Location_Longitude",
                table: "Users",
                type: "float",
                nullable: true);
        }
    }
}
