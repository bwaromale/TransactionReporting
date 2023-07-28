using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionReportingAPI.Migrations
{
    public partial class UpdateCustomerAddedCustomerRef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerRef",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerRef",
                table: "Customers");
        }
    }
}
