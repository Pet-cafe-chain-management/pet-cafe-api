using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_Product_2025_09_25_22_53 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_products_product_categories_product_category_id",
                table: "products");

            migrationBuilder.RenameColumn(
                name: "product_category_id",
                table: "products",
                newName: "category_id");

            migrationBuilder.RenameIndex(
                name: "i_x_products_product_category_id_is_active",
                table: "products",
                newName: "i_x_products_category_id_is_active");

            migrationBuilder.AddColumn<string>(
                name: "avatar_url",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "employees",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "avatar_url",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "f_k_products_product_categories_category_id",
                table: "products",
                column: "category_id",
                principalTable: "product_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_products_product_categories_category_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "avatar_url",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "email",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "avatar_url",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "email",
                table: "customers");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "products",
                newName: "product_category_id");

            migrationBuilder.RenameIndex(
                name: "i_x_products_category_id_is_active",
                table: "products",
                newName: "i_x_products_product_category_id_is_active");

            migrationBuilder.AddForeignKey(
                name: "f_k_products_product_categories_product_category_id",
                table: "products",
                column: "product_category_id",
                principalTable: "product_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
