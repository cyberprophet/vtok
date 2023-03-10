using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShareInvest.Server.Data.Migrations
{
    public partial class CreateOPT10081 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OPT10081",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Date = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Current = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Volume = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Start = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    High = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Low = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Revise = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ReviseRate = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    MainCategory = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    SubCategory = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    StockInfo = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ReviseEvent = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Close = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OPT10081", x => new { x.Code, x.Date });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OPT10081");
        }
    }
}
