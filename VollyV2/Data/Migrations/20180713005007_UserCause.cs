using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace VollyV2.Data.Migrations
{
    public partial class UserCause : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCauses",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    CauseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCauses", x => new { x.UserId, x.CauseId });
                    table.ForeignKey(
                        name: "FK_UserCauses_Causes_CauseId",
                        column: x => x.CauseId,
                        principalTable: "Causes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCauses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCauses_CauseId",
                table: "UserCauses",
                column: "CauseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCauses");
        }
    }
}
