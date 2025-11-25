using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedEmailVerificationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "InboxMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderAvatarInitials = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Excerpt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsSupportTicket = table.Column<bool>(type: "bit", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboxMessage_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateSubmitted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProjectDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SkillsRequired = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRequest_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttachedDocument",
                columns: table => new
                {
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DownloadUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachedDocument", x => new { x.ServiceRequestId, x.Id });
                    table.ForeignKey(
                        name: "FK_AttachedDocument_ServiceRequest_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Proposal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CraftsmanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CraftsmanAvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CraftsmanRating = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Excerpt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProposalUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proposal_ServiceRequest_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Proposal_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationTokens_CraftmanId",
                table: "EmailVerificationTokens",
                column: "CraftmanId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationTokens_CustomerId",
                table: "EmailVerificationTokens",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessage_CustomerId",
                table: "InboxMessage",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_CustomerId",
                table: "Proposal",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_ServiceRequestId",
                table: "Proposal",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequest_CustomerId",
                table: "ServiceRequest",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationTokens_Users_CraftmanId",
                table: "EmailVerificationTokens",
                column: "CraftmanId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationTokens_Users_CustomerId",
                table: "EmailVerificationTokens",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationTokens_Users_CraftmanId",
                table: "EmailVerificationTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationTokens_Users_CustomerId",
                table: "EmailVerificationTokens");

            migrationBuilder.DropTable(
                name: "AttachedDocument");

            migrationBuilder.DropTable(
                name: "InboxMessage");

            migrationBuilder.DropTable(
                name: "Proposal");

            migrationBuilder.DropTable(
                name: "ServiceRequest");

            migrationBuilder.DropIndex(
                name: "IX_EmailVerificationTokens_CraftmanId",
                table: "EmailVerificationTokens");

            migrationBuilder.DropIndex(
                name: "IX_EmailVerificationTokens_CustomerId",
                table: "EmailVerificationTokens");

            migrationBuilder.DropColumn(
                name: "CraftmanId",
                table: "EmailVerificationTokens");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "EmailVerificationTokens");
        }
    }
}
