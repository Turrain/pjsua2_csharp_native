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
                    AgentId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    LLM_Model = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    LLM_Temperature = table.Column<float>(type: "REAL", nullable: false, defaultValue: 0.7f),
                    LLM_MaxTokens = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 512),
                    LLM_OllamaEndpoint = table.Column<string>(type: "TEXT", nullable: false),
                    LLM_Parameters = table.Column<string>(type: "TEXT", nullable: true),
                    Whisper_Endpoint = table.Column<string>(type: "TEXT", nullable: false),
                    Whisper_Language = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false, defaultValue: "en"),
                    Whisper_Timeout = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 30),
                    Whisper_EnableTranslation = table.Column<bool>(type: "INTEGER", nullable: false),
                    Auralis_Endpoint = table.Column<string>(type: "TEXT", nullable: false),
                    Auralis_ApiKey = table.Column<string>(type: "TEXT", nullable: false),
                    Auralis_Timeout = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 30),
                    Auralis_EnableAnalytics = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sender = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SipAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Domain = table.Column<string>(type: "TEXT", nullable: false),
                    RegistrarUri = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    AgentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SipAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SipAccounts_AgentConfigs_AgentId",
                        column: x => x.AgentId,
                        principalTable: "AgentConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SipCalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CallId = table.Column<int>(type: "INTEGER", nullable: false),
                    RemoteUri = table.Column<string>(type: "TEXT", maxLength: 511, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
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
                name: "IX_SipAccounts_AgentId",
                table: "SipAccounts",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_SipCalls_SipAccountId",
                table: "SipCalls",
                column: "SipAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "SipCalls");

            migrationBuilder.DropTable(
                name: "SipAccounts");

            migrationBuilder.DropTable(
                name: "AgentConfigs");
        }
    }
}
