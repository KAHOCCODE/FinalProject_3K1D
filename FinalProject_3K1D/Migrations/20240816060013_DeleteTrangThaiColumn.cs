using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_3K1D.Migrations
{
    /// <inheritdoc />
    public partial class DeleteTrangThaiColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "LichChieu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrangThai",
                table: "LichChieu",
                type: "int",
                nullable: false,
                defaultValueSql: "('0')");
        }
    }
}
