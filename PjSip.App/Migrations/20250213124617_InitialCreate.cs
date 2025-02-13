using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PjSip.App.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgentConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AgentId = table.Column<string>(type: "TEXT", nullable: false),
                    LLMConfig = table.Column<string>(type: "TEXT", nullable: false),
                    AuralisEndpoint = table.Column<string>(type: "TEXT", nullable: false),
                    WhisperEndpoint = table.Column<string>(type: "TEXT", nullable: false),
                    OllamaEndpoint = table.Column<string>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SipAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountId = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Domain = table.Column<string>(type: "TEXT", nullable: false),
                    RegistrarUri = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SipAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SipCalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CallId = table.Column<int>(type: "INTEGER", nullable: false),
                    RemoteUri = table.Column<string>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    SipAccountId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SipCalls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SipCalls_SipAccounts_SipAccountId",
                        column: x => x.SipAccountId,
                        principalTable: "SipAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SipAccounts_AccountId",
                table: "SipAccounts",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SipCalls_SipAccountId",
                table: "SipCalls",
                column: "SipAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentConfigs");

            migrationBuilder.DropTable(
                name: "SipCalls");

            migrationBuilder.DropTable(
                name: "SipAccounts");
        }
    }
}
