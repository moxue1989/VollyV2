using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace VollyV2.Data.Migrations
{
    public partial class removefamilyfriendly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FamilyFriendly",
                table: "Opportunities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FamilyFriendly",
                table: "Opportunities",
                nullable: false,
                defaultValue: false);
        }
    }
}
