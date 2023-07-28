using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionReportingAPI.Migrations
{
    public partial class UpdateCustomerAddedCustomerRefUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CustomerRef",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerRef",
                table: "Customers",
                column: "CustomerRef",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_CustomerRef",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerRef",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
