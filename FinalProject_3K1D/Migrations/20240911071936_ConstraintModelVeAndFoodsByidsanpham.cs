using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_3K1D.Migrations
{
    /// <inheritdoc />
    public partial class ConstraintModelVeAndFoodsByidsanpham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdSanPham",
                table: "Ve",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SanPhamNavigationIdSanPham",
                table: "Ve",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ve_SanPhamNavigationIdSanPham",
                table: "Ve",
                column: "SanPhamNavigationIdSanPham");

            migrationBuilder.AddForeignKey(
                name: "FK_Ve_Foods_SanPhamNavigationIdSanPham",
                table: "Ve",
                column: "SanPhamNavigationIdSanPham",
                principalTable: "Foods",
                principalColumn: "IdSanPham");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ve_Foods_SanPhamNavigationIdSanPham",
                table: "Ve");

            migrationBuilder.DropIndex(
                name: "IX_Ve_SanPhamNavigationIdSanPham",
                table: "Ve");

            migrationBuilder.DropColumn(
                name: "IdSanPham",
                table: "Ve");

            migrationBuilder.DropColumn(
                name: "SanPhamNavigationIdSanPham",
                table: "Ve");
        }
    }
}
