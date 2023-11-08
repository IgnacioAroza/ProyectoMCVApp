using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFinalApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "stockId",
                table: "productos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "stockId",
                table: "productos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
