using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpdeskRepository.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationUpsert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastAt",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastMessage",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UnreadCount",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "LastMessage",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UnreadCount",
                table: "Notifications");
        }
    }
}
