using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace VollyV2.Data.Migrations
{
    public partial class RemoveNotNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHours_Applications_ApplicationId",
                table: "VolunteerHours");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerHours_ApplicationId",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "VolunteerHours");

            migrationBuilder.AddColumn<int>(
                name: "VolunteerHoursId",
                table: "Applications",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_VolunteerHoursId",
                table: "Applications",
                column: "VolunteerHoursId",
                unique: true,
                filter: "[VolunteerHoursId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_VolunteerHours_VolunteerHoursId",
                table: "Applications",
                column: "VolunteerHoursId",
                principalTable: "VolunteerHours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_VolunteerHours_VolunteerHoursId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_VolunteerHoursId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "VolunteerHoursId",
                table: "Applications");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "VolunteerHours",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerHours_ApplicationId",
                table: "VolunteerHours",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerHours_Applications_ApplicationId",
                table: "VolunteerHours",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
