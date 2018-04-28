using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace VollyV2.Data.Migrations
{
    public partial class AddCauses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CauseId",
                table: "Organizations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Causes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Causes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CauseId",
                table: "Organizations",
                column: "CauseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_Causes_CauseId",
                table: "Organizations",
                column: "CauseId",
                principalTable: "Causes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_Causes_CauseId",
                table: "Organizations");

            migrationBuilder.DropTable(
                name: "Causes");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_CauseId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CauseId",
                table: "Organizations");
        }
    }
}
