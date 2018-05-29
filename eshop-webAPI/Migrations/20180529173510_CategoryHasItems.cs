using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace eshopAPI.Migrations
{
    public partial class CategoryHasItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_SubCategories_SubCategoryID",
                table: "Items");

            migrationBuilder.AlterColumn<long>(
                name: "SubCategoryID",
                table: "Items",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "CategoryID",
                table: "Items",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryID",
                table: "Items",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryID",
                table: "Items",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_SubCategories_SubCategoryID",
                table: "Items",
                column: "SubCategoryID",
                principalTable: "SubCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryID",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_SubCategories_SubCategoryID",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CategoryID",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Items");

            migrationBuilder.AlterColumn<long>(
                name: "SubCategoryID",
                table: "Items",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_SubCategories_SubCategoryID",
                table: "Items",
                column: "SubCategoryID",
                principalTable: "SubCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
