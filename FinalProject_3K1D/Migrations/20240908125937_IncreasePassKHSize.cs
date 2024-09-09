using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_3K1D.Migrations
{
    /// <inheritdoc />
    public partial class IncreasePassKHSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
           name: "PassKH",
           table: "KhachHang",
           type: "varchar(100)",  // Thay đổi kích thước cột ở đây
           maxLength: 100,
           nullable: true,
           oldClrType: typeof(string),
           oldType: "varchar(50)",
           oldMaxLength: 50);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
           name: "PassKH",
           table: "KhachHang",
           type: "varchar(50)",
           maxLength: 50,
           nullable: true,
           oldClrType: typeof(string),
           oldType: "varchar(100)",
           oldMaxLength: 100);
        }
    }
}
