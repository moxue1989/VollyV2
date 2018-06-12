using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace VollyV2.Data.Migrations
{
    public partial class occurrenceManage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Occurrences_Opportunities_OpportunityId",
                table: "Occurrences");

            migrationBuilder.AlterColumn<int>(
                name: "OpportunityId",
                table: "Occurrences",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Occurrences_Opportunities_OpportunityId",
                table: "Occurrences",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Occurrences_Opportunities_OpportunityId",
                table: "Occurrences");

            migrationBuilder.AlterColumn<int>(
                name: "OpportunityId",
                table: "Occurrences",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Occurrences_Opportunities_OpportunityId",
                table: "Occurrences",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
