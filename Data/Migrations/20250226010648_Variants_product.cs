using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Variants_product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cata_Product_Product_product_id",
                table: "Cata_Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Size_Product_Cata_Product_variants_product_id",
                table: "Size_Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cata_Product",
                table: "Cata_Product");

            migrationBuilder.RenameTable(
                name: "Cata_Product",
                newName: "Variants_product");

            migrationBuilder.RenameIndex(
                name: "IX_Cata_Product_product_id",
                table: "Variants_product",
                newName: "IX_Variants_product_product_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Variants_product",
                table: "Variants_product",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Size_Product_Variants_product_variants_product_id",
                table: "Size_Product",
                column: "variants_product_id",
                principalTable: "Variants_product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Variants_product_Product_product_id",
                table: "Variants_product",
                column: "product_id",
                principalTable: "Product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Size_Product_Variants_product_variants_product_id",
                table: "Size_Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Variants_product_Product_product_id",
                table: "Variants_product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Variants_product",
                table: "Variants_product");

            migrationBuilder.RenameTable(
                name: "Variants_product",
                newName: "Cata_Product");

            migrationBuilder.RenameIndex(
                name: "IX_Variants_product_product_id",
                table: "Cata_Product",
                newName: "IX_Cata_Product_product_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cata_Product",
                table: "Cata_Product",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cata_Product_Product_product_id",
                table: "Cata_Product",
                column: "product_id",
                principalTable: "Product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Size_Product_Cata_Product_variants_product_id",
                table: "Size_Product",
                column: "variants_product_id",
                principalTable: "Cata_Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
