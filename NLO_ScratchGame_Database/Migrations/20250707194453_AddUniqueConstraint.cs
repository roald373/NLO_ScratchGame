using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NLO_ScratchGame_Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ScratchCells_ScratchedByUserId",
                table: "ScratchCells",
                column: "ScratchedByUserId",
                unique: true,
                filter: "[ScratchedByUserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScratchCells_ScratchedByUserId",
                table: "ScratchCells");
        }
    }
}
