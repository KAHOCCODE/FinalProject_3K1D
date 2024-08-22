using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_3K1D.Migrations
{
    public partial class AddIdKhuyenMaiToKhachHang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột idKhuyenMai vào bảng KhachHang
            migrationBuilder.AddColumn<int>(
                name: "idKhuyenMai",
                table: "KhachHang",
                type: "int",
                nullable: true);

            // Tạo chỉ mục cho cột idKhuyenMai
            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_idKhuyenMai",
                table: "KhachHang",
                column: "idKhuyenMai");

            // Thêm khóa ngoại cho cột idKhuyenMai, liên kết đến bảng KhuyenMai
            migrationBuilder.AddForeignKey(
                name: "FK_KhachHang_KhuyenMai_idKhuyenMai",
                table: "KhachHang",
                column: "idKhuyenMai",
                principalTable: "KhuyenMai",
                principalColumn: "idKhuyenMai",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa khóa ngoại nếu có
            migrationBuilder.DropForeignKey(
                name: "FK_KhachHang_KhuyenMai_idKhuyenMai",
                table: "KhachHang");

            // Xóa chỉ mục nếu có
            migrationBuilder.DropIndex(
                name: "IX_KhachHang_idKhuyenMai",
                table: "KhachHang");

            // Xóa cột idKhuyenMai khỏi bảng KhachHang
            migrationBuilder.DropColumn(
                name: "idKhuyenMai",
                table: "KhachHang");
        }
    }
}
