using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace eshopAPI.Migrations
{
    public partial class FixDiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_Categories_CategoryID",
                table: "Discounts");

            migrationBuilder.AlterColumn<long>(
                name: "CategoryID",
                table: "Discounts",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<bool>(
                name: "IsPercentages",
                table: "Discounts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ItemID",
                table: "Discounts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_ItemID",
                table: "Discounts",
                column: "ItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_Categories_CategoryID",
                table: "Discounts",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_Items_ItemID",
                table: "Discounts",
                column: "ItemID",
                principalTable: "Items",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_Categories_CategoryID",
                table: "Discounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_Items_ItemID",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_ItemID",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "IsPercentages",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "ItemID",
                table: "Discounts");

            migrationBuilder.AlterColumn<long>(
                name: "CategoryID",
                table: "Discounts",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_Categories_CategoryID",
                table: "Discounts",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
