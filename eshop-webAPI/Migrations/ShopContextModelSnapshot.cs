﻿// <auto-generated />
using eshopAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace eshopAPI.Migrations
{
    [DbContext(typeof(ShopContext))]
    partial class ShopContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("eshopAPI.Models.Attribute", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("ID");

                    b.ToTable("Attributes");
                });

            modelBuilder.Entity("eshopAPI.Models.AttributeValue", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AttributeID");

                    b.Property<long?>("ItemID");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("ID");

                    b.HasIndex("AttributeID");

                    b.HasIndex("ItemID");

                    b.ToTable("AttributeValue");
                });

            modelBuilder.Entity("eshopAPI.Models.Cart", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("eshopAPI.Models.CartItem", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("CartID");

                    b.Property<int>("Count");

                    b.Property<long>("ItemID");

                    b.HasKey("ID");

                    b.HasIndex("CartID");

                    b.HasIndex("ItemID");

                    b.ToTable("CartItem");
                });

            modelBuilder.Entity("eshopAPI.Models.Category", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<long>("SubCategoryID");

                    b.HasKey("ID");

                    b.HasIndex("SubCategoryID");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("eshopAPI.Models.Item", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("CategoryID");

                    b.Property<DateTime>("CreateDate");

                    b.Property<DateTime>("DeleteDate");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(5000);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<decimal>("Price");

                    b.Property<string>("SKU")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("eshopAPI.Models.Order", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDate");

                    b.Property<Guid>("OrderNumber");

                    b.Property<int>("Status");

                    b.Property<long>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("eshopAPI.Models.OrderItem", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Count");

                    b.Property<long>("ItemID");

                    b.Property<long?>("OrderID");

                    b.Property<decimal>("Price");

                    b.HasKey("ID");

                    b.HasIndex("ItemID");

                    b.HasIndex("OrderID");

                    b.ToTable("OrderItem");
                });

            modelBuilder.Entity("eshopAPI.Models.Profile", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<long>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("eshopAPI.Models.User", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Approved");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int>("Role");

                    b.HasKey("ID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("eshopAPI.Models.AttributeValue", b =>
                {
                    b.HasOne("eshopAPI.Models.Attribute", "Attribute")
                        .WithMany()
                        .HasForeignKey("AttributeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eshopAPI.Models.Item")
                        .WithMany("Attrbutes")
                        .HasForeignKey("ItemID");
                });

            modelBuilder.Entity("eshopAPI.Models.Cart", b =>
                {
                    b.HasOne("eshopAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eshopAPI.Models.CartItem", b =>
                {
                    b.HasOne("eshopAPI.Models.Cart")
                        .WithMany("Items")
                        .HasForeignKey("CartID");

                    b.HasOne("eshopAPI.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eshopAPI.Models.Category", b =>
                {
                    b.HasOne("eshopAPI.Models.Category", "SubCategory")
                        .WithMany()
                        .HasForeignKey("SubCategoryID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eshopAPI.Models.Item", b =>
                {
                    b.HasOne("eshopAPI.Models.Category")
                        .WithMany("Items")
                        .HasForeignKey("CategoryID");
                });

            modelBuilder.Entity("eshopAPI.Models.Order", b =>
                {
                    b.HasOne("eshopAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eshopAPI.Models.OrderItem", b =>
                {
                    b.HasOne("eshopAPI.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eshopAPI.Models.Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderID");
                });

            modelBuilder.Entity("eshopAPI.Models.Profile", b =>
                {
                    b.HasOne("eshopAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
