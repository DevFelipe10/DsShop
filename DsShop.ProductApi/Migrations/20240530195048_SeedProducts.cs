using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DsShop.ProductApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO Products(Name, Price, Description, Stock, ImageURL, CategoryId) " +
                "VALUES('Caderno', 7.55, 'Caderno', 10, 'caderno.jpg', 1)");
            mb.Sql("INSERT INTO Products(Name, Price, Description, Stock, ImageURL, CategoryId) " +
                "VALUES('Régua', 5.00, 'Régua', 5, 'regua.jpg', 1)");
            mb.Sql("INSERT INTO Products(Name, Price, Description, Stock, ImageURL, CategoryId) " +
                "VALUES('Clips', 1.32, 'Clips', 100, 'clips.jpg', 2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("DELETE FROM Products");
        }
    }
}
