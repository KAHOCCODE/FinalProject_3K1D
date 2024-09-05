using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_3K1D.Migrations
{
    /// <inheritdoc />
    public partial class AddGiaHoanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiaHoans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThoiGianHoan = table.Column<int>(type: "int", nullable: false),
                    PhanTramHoan = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaHoans", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiaHoans");
        }
    }
}
