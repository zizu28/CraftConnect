using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class ResetPasswordTokenEntity : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "AuditLogs",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
					EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
					Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
					EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
					IpAddress = table.Column<string>(type: "nvarchar(45)", nullable: false),
					OldValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
					NewValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AuditLogs", x => x.Id);
				});

			// 3. Create ResetPasswordTokens with RESTRICT (The Fix)
			migrationBuilder.CreateTable(
				name: "ResetPasswordTokens",
				columns: table => new
				{
					ResetPasswordTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TokenValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
					ExpiresOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ResetPasswordTokens", x => x.ResetPasswordTokenId);
					table.ForeignKey(
						name: "FK_ResetPasswordTokens_Users_UserId",
						column: x => x.UserId,
						principalTable: "Users",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_ResetPasswordTokens_UserId",
				table: "ResetPasswordTokens",
				column: "UserId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			// Reverse the operations
			migrationBuilder.DropTable(
				name: "AuditLogs");

			migrationBuilder.DropTable(
				name: "ResetPasswordTokens");

			// Restore the old columns in EmailVerificationTokens
			migrationBuilder.AddColumn<Guid>(
				name: "CraftmanId",
				table: "EmailVerificationTokens",
				type: "uniqueidentifier",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			migrationBuilder.AddColumn<Guid>(
				name: "CustomerId",
				table: "EmailVerificationTokens",
				type: "uniqueidentifier",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			migrationBuilder.CreateIndex(
				name: "IX_EmailVerificationTokens_CraftmanId",
				table: "EmailVerificationTokens",
				column: "CraftmanId");

			migrationBuilder.CreateIndex(
				name: "IX_EmailVerificationTokens_CustomerId",
				table: "EmailVerificationTokens",
				column: "CustomerId");

			migrationBuilder.AddForeignKey(
				name: "FK_EmailVerificationTokens_Users_CraftmanId",
				table: "EmailVerificationTokens",
				column: "CraftmanId",
				principalTable: "Users",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_EmailVerificationTokens_Users_CustomerId",
				table: "EmailVerificationTokens",
				column: "CustomerId",
				principalTable: "Users",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}