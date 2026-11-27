using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRanking.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchRequestId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "Matches",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_RequestId",
                table: "Matches",
                column: "RequestId",
                unique: true,
                filter: "[RequestId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Matches_RequestId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Matches");
        }
    }
}
