using Microsoft.EntityFrameworkCore.Migrations;

namespace MyLinks.Web.Migrations
{
    public partial class dataseed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Tech" });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "CategoryId", "Url" },
                values: new object[] { 1, 1, "http://uneurldetest.com" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Links",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
