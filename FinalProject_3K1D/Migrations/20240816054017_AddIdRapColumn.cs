using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_3K1D.Migrations
{
    public partial class AddIdRapColumn : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdRap",
                table: "LichChieu",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdRapNavigationIdRap",
                table: "LichChieu",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichChieu_IdRapNavigationIdRap",
                table: "LichChieu",
                column: "IdRapNavigationIdRap");

            migrationBuilder.AddForeignKey(
                name: "FK_LichChieu_Rap_IdRapNavigationIdRap",
                table: "LichChieu",
                column: "IdRapNavigationIdRap",
                principalTable: "Rap",
                principalColumn: "idRap");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichChieu_Rap_IdRapNavigationIdRap",
                table: "LichChieu");

            migrationBuilder.DropIndex(
                name: "IX_LichChieu_IdRapNavigationIdRap",
                table: "LichChieu");

            migrationBuilder.DropColumn(
                name: "IdRap",
                table: "LichChieu");

            migrationBuilder.DropColumn(
                name: "IdRapNavigationIdRap",
                table: "LichChieu");
        }
    }
}
