using Microsoft.EntityFrameworkCore.Migrations;

namespace AngularEshop.DataLayer.Migrations
{
    public partial class ModifyProductOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_orders_OrderId",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_OrderId",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "orders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                table: "orders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderId",
                table: "orders",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_orders_OrderId",
                table: "orders",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
