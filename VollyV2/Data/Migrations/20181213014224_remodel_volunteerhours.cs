using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace VollyV2.Data.Migrations
{
    public partial class remodel_volunteerhours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "OpportunityName",
                table: "VolunteerHours",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpportunityName",
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
    }
}
