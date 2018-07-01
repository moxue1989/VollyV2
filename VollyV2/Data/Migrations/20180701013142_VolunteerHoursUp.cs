using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace VollyV2.Data.Migrations
{
    public partial class VolunteerHoursUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHours_Occurrences_OccurrenceId",
                table: "VolunteerHours");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerHours_OccurrenceId",
                table: "VolunteerHours");

            migrationBuilder.RenameColumn(
                name: "OccurrenceId",
                table: "VolunteerHours",
                newName: "ApplicationId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHours_Applications_ApplicationId",
                table: "VolunteerHours");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerHours_ApplicationId",
                table: "VolunteerHours");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "VolunteerHours",
                newName: "OccurrenceId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerHours_OccurrenceId",
                table: "VolunteerHours",
                column: "OccurrenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerHours_Occurrences_OccurrenceId",
                table: "VolunteerHours",
                column: "OccurrenceId",
                principalTable: "Occurrences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
