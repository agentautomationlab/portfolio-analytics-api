using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace portfolio_analytics_api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalyticsEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PageTitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Referrer = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    UtmSource = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UtmMedium = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UtmCampaign = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Browser = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Os = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Device = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ScreenResolution = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ViewportWidth = table.Column<int>(type: "integer", nullable: true),
                    ViewportHeight = table.Column<int>(type: "integer", nullable: true),
                    LoadTime = table.Column<int>(type: "integer", nullable: true),
                    CustomData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_EventType",
                table: "AnalyticsEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_PageUrl_Timestamp",
                table: "AnalyticsEvents",
                columns: new[] { "PageUrl", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_SessionId",
                table: "AnalyticsEvents",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_Timestamp",
                table: "AnalyticsEvents",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalyticsEvents");
        }
    }
}
