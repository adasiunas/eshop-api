using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace eshopAPI.Migrations
{
    public partial class CategoriesRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentID",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryID",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentID",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "Items",
                newName: "SubCategoryID");

            migrationBuilder.RenameIndex(
                name: "IX_Items_CategoryID",
                table: "Items",
                newName: "IX_Items_SubCategoryID");

            migrationBuilder.AlterColumn<string>(
                name: "URL",
                table: "ItemPicture",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "SubCategory",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryID = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubCategory_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryID",
                table: "SubCategory",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_SubCategory_SubCategoryID",
                table: "Items",
                column: "SubCategoryID",
                principalTable: "SubCategory",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_SubCategory_SubCategoryID",
                table: "Items");

            migrationBuilder.DropTable(
                name: "SubCategory");

            migrationBuilder.RenameColumn(
                name: "SubCategoryID",
                table: "Items",
                newName: "CategoryID");

            migrationBuilder.RenameIndex(
                name: "IX_Items_SubCategoryID",
                table: "Items",
                newName: "IX_Items_CategoryID");

            migrationBuilder.AlterColumn<string>(
                name: "URL",
                table: "ItemPicture",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 500);

            migrationBuilder.AddColumn<long>(
                name: "ParentID",
                table: "Categories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentID",
                table: "Categories",
                column: "ParentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentID",
                table: "Categories",
                column: "ParentID",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryID",
                table: "Items",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
