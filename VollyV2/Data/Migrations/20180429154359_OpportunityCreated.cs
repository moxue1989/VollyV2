using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace VollyV2.Data.Migrations
{
    public partial class OpportunityCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Opportunities",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_CreatedByUserId",
                table: "Opportunities",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_AspNetUsers_CreatedByUserId",
                table: "Opportunities",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_AspNetUsers_CreatedByUserId",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_CreatedByUserId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Opportunities");
        }
    }
}
