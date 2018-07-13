using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace VollyV2.Data.Migrations
{
    public partial class addCommunityToOpportunity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommunityId",
                table: "Opportunities",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_CommunityId",
                table: "Opportunities",
                column: "CommunityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Communities_CommunityId",
                table: "Opportunities",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Communities_CommunityId",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_CommunityId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "Opportunities");
        }
    }
}
