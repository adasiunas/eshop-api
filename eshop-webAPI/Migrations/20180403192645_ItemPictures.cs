using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace eshopAPI.Migrations
{
    public partial class ItemPictures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_SubCategoryID",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_SubCategoryID",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SubCategoryID",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress",
                table: "Orders",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ParentID",
                table: "Categories",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AddressID",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    City = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Postcode = table.Column<string>(nullable: false),
                    Street = table.Column<string>(nullable: false),
                    Surname = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ItemPicture",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<long>(nullable: true),
                    URL = table.Column<string>(nullable: false, maxLength: 500)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPicture", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemPicture_Items_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentID",
                table: "Categories",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AddressID",
                table: "AspNetUsers",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPicture_ItemID",
                table: "ItemPicture",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPicture_URL",
                table: "ItemPicture",
                column: "URL",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Address_AddressID",
                table: "AspNetUsers",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentID",
                table: "Categories",
                column: "ParentID",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Address_AddressID",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentID",
                table: "Categories");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "ItemPicture");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentID",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AddressID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DeliveryAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "AddressID",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<long>(
                name: "SubCategoryID",
                table: "Categories",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SubCategoryID",
                table: "Categories",
                column: "SubCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_SubCategoryID",
                table: "Categories",
                column: "SubCategoryID",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
