using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OrderManagement.Infrastructure.Persistence;

#nullable disable

namespace OrderManagement.Infrastructure.Migrations;

[DbContext(typeof(OrdersDbContext))]
[Migration("20260914224500_StoreUnitPriceAsText")]
public partial class StoreUnitPriceAsText : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            CREATE TABLE "OrderItems_new" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_OrderItems_new" PRIMARY KEY,
                "OrderId" TEXT NOT NULL,
                "ProductName" TEXT NOT NULL,
                "Quantity" INTEGER NOT NULL,
                "UnitPrice" TEXT NOT NULL,
                CONSTRAINT "FK_OrderItems_new_Orders_OrderId"
                    FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
            );

            INSERT INTO "OrderItems_new" ("Id", "OrderId", "ProductName", "Quantity", "UnitPrice")
            SELECT "Id", "OrderId", "ProductName", "Quantity", CAST("UnitPrice" AS TEXT)
            FROM "OrderItems";

            DROP TABLE "OrderItems";
            ALTER TABLE "OrderItems_new" RENAME TO "OrderItems";
            CREATE INDEX "IX_OrderItems_OrderId" ON "OrderItems" ("OrderId");
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            CREATE TABLE "OrderItems_old" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_OrderItems_old" PRIMARY KEY,
                "OrderId" TEXT NOT NULL,
                "ProductName" TEXT NOT NULL,
                "Quantity" INTEGER NOT NULL,
                "UnitPrice" REAL NOT NULL,
                CONSTRAINT "FK_OrderItems_old_Orders_OrderId"
                    FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
            );

            INSERT INTO "OrderItems_old" ("Id", "OrderId", "ProductName", "Quantity", "UnitPrice")
            SELECT "Id", "OrderId", "ProductName", "Quantity", CAST("UnitPrice" AS REAL)
            FROM "OrderItems";

            DROP TABLE "OrderItems";
            ALTER TABLE "OrderItems_old" RENAME TO "OrderItems";
            CREATE INDEX "IX_OrderItems_OrderId" ON "OrderItems" ("OrderId");
            """);
    }
}
