using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace VollyV2.Data.Migrations
{
    public partial class Moreopportunityfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicationDeadline",
                table: "Opportunities",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "Opportunities",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "FamilyFriendly",
                table: "Opportunities",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Openings",
                table: "Opportunities",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationDeadline",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "FamilyFriendly",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "Openings",
                table: "Opportunities");
        }
    }
}
