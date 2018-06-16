using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace VollyV2.Data.Migrations
{
    public partial class Refactor_DB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OccurrenceApplications_Applications_ApplicationId",
                table: "OccurrenceApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_OccurrenceApplications_Occurrences_OccurrenceId",
                table: "OccurrenceApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OccurrenceApplications",
                table: "OccurrenceApplications");

            migrationBuilder.DropColumn(
                name: "ApplicationDeadline",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "Opportunities");

            migrationBuilder.RenameTable(
                name: "OccurrenceApplications",
                newName: "ApplicationsOccurrence");

            migrationBuilder.RenameIndex(
                name: "IX_OccurrenceApplications_OccurrenceId",
                table: "ApplicationsOccurrence",
                newName: "IX_ApplicationsOccurrence_OccurrenceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationsOccurrence",
                table: "ApplicationsOccurrence",
                columns: new[] { "ApplicationId", "OccurrenceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationsOccurrence_Applications_ApplicationId",
                table: "ApplicationsOccurrence",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationsOccurrence_Occurrences_OccurrenceId",
                table: "ApplicationsOccurrence",
                column: "OccurrenceId",
                principalTable: "Occurrences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationsOccurrence_Applications_ApplicationId",
                table: "ApplicationsOccurrence");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationsOccurrence_Occurrences_OccurrenceId",
                table: "ApplicationsOccurrence");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationsOccurrence",
                table: "ApplicationsOccurrence");

            migrationBuilder.RenameTable(
                name: "ApplicationsOccurrence",
                newName: "OccurrenceApplications");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationsOccurrence_OccurrenceId",
                table: "OccurrenceApplications",
                newName: "IX_OccurrenceApplications_OccurrenceId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicationDeadline",
                table: "Opportunities",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "Opportunities",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "Opportunities",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_OccurrenceApplications",
                table: "OccurrenceApplications",
                columns: new[] { "ApplicationId", "OccurrenceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OccurrenceApplications_Applications_ApplicationId",
                table: "OccurrenceApplications",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OccurrenceApplications_Occurrences_OccurrenceId",
                table: "OccurrenceApplications",
                column: "OccurrenceId",
                principalTable: "Occurrences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
