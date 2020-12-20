using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChainStore.DataAccessLayerImpl.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Books",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false),
                    ExpirationTime = table.Column<DateTimeOffset>(nullable: false),
                    ReserveDaysCount = table.Column<int>(nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_Books", x => x.Id));

            migrationBuilder.CreateTable(
                "Clients",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Balance = table.Column<double>(nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_Clients", x => x.Id));

            migrationBuilder.CreateTable(
                "Purchases",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false),
                    PriceAtPurchaseMoment = table.Column<double>(nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_Purchases", x => x.Id));

            migrationBuilder.CreateTable(
                "Roles",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoleName = table.Column<string>(nullable: true),
                },
                constraints: table => table.PrimaryKey("PK_Roles", x => x.Id));

            migrationBuilder.CreateTable(
                "Stores",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Profit = table.Column<double>(nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_Stores", x => x.Id));

            migrationBuilder.CreateTable(
                "UserRoles",
                table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId }));

            migrationBuilder.CreateTable(
                "Users",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    HashedPassword = table.Column<string>(nullable: true),
                    ClientId = table.Column<Guid>(nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_Users", x => x.Id));

            migrationBuilder.CreateTable(
                "Categories",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    StoreDbModelId = table.Column<Guid>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        "FK_Categories_Stores_StoreDbModelId",
                        x => x.StoreDbModelId,
                        "Stores",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Products",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PriceInUAH = table.Column<double>(nullable: false),
                    ProductStatus = table.Column<int>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        "FK_Products_Categories_CategoryId",
                        x => x.CategoryId,
                        "Categories",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "StoreCategoryRelation",
                table => new
                {
                    StoreDbModelId = table.Column<Guid>(nullable: false),
                    CategoryDbModelId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreCategoryRelation", x => new { x.StoreDbModelId, x.CategoryDbModelId });
                    table.ForeignKey(
                        "FK_StoreCategoryRelation_Categories_CategoryDbModelId",
                        x => x.CategoryDbModelId,
                        "Categories",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_StoreCategoryRelation_Stores_StoreDbModelId",
                        x => x.StoreDbModelId,
                        "Stores",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "StoreProductRelation",
                table => new
                {
                    StoreDbModelId = table.Column<Guid>(nullable: false),
                    ProductDbModelId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreProductRelation", x => new { x.StoreDbModelId, x.ProductDbModelId });
                    table.ForeignKey(
                        "FK_StoreProductRelation_Products_ProductDbModelId",
                        x => x.ProductDbModelId,
                        "Products",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_StoreProductRelation_Stores_StoreDbModelId",
                        x => x.StoreDbModelId,
                        "Stores",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                "Categories",
                new[] { "Id", "Name", "StoreDbModelId" },
                new object[,]
                {
                    { new Guid("080917f2-e2fa-4581-a7c2-743b259852ef"), "Laptop", null },
                    { new Guid("1696b27d-8452-458a-994b-fdeef9cff690"), "Mouse", null },
                    { new Guid("f13e35c0-684c-470c-9f9d-0f35867a1bda"), "USB", null },
                });

            migrationBuilder.InsertData(
                "Stores",
                new[] { "Id", "Location", "Name", "Profit" },
                new object[]
                {
                    new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"), "10 Pandora Street", "Shields and Weapons", 0.0,
                });

            migrationBuilder.InsertData(
                "Products",
                new[] { "Id", "CategoryId", "Name", "PriceInUAH", "ProductStatus" },
                new object[,]
                {
                    {
                        new Guid("e15a240e-bced-4458-a6b6-347e06abe88c"),
                        new Guid("080917f2-e2fa-4581-a7c2-743b259852ef"), "HP 450 G1", 20000.0, 0,
                    },
                    {
                        new Guid("76946bca-9fb0-4eff-8b46-a4949e256a1c"),
                        new Guid("080917f2-e2fa-4581-a7c2-743b259852ef"), "HP 450 G2", 30000.0, 0,
                    },
                    {
                        new Guid("6e367c9e-2c95-4552-9599-820a15c29e7a"),
                        new Guid("080917f2-e2fa-4581-a7c2-743b259852ef"), "HP 450 G3", 40000.0, 0,
                    },
                    {
                        new Guid("d713dc3b-92a3-45cc-befb-f70a82c0323c"),
                        new Guid("080917f2-e2fa-4581-a7c2-743b259852ef"), "HP 450 G4", 50000.0, 0,
                    },
                    {
                        new Guid("004f7b54-d482-4418-b6f4-ded434ee2d76"),
                        new Guid("080917f2-e2fa-4581-a7c2-743b259852ef"), "HP 850 G5", 60000.0, 0,
                    },
                    {
                        new Guid("d10d49a3-dbfa-4946-a3bd-66f93b62e5e1"),
                        new Guid("080917f2-e2fa-4581-a7c2-743b259852ef"), "HP 450 G1", 20000.0, 0,
                    },
                    {
                        new Guid("5445426b-308e-4344-9a73-ca39cb5bc674"),
                        new Guid("080917f2-e2fa-4581-a7c2-743b259852ef"), "HP 450 G1", 20000.0, 0,
                    },
                    {
                        new Guid("611c8fab-b1c5-485e-a8ab-d078deb9f2d8"),
                        new Guid("080917f2-e2fa-4581-a7c2-743b259852ef"), "HP 450 G1", 20000.0, 0,
                    },
                    {
                        new Guid("c8baed60-1600-483d-9d3e-6004cabc19c1"),
                        new Guid("080917f2-e2fa-4581-a7c2-743b259852ef"), "HP 450 G1", 20000.0, 0,
                    },
                    {
                        new Guid("437de9d5-f77e-43a3-9b1c-490e78a782a7"),
                        new Guid("1696b27d-8452-458a-994b-fdeef9cff690"), "LogTech G12", 1000.0, 0,
                    },
                    {
                        new Guid("54da41e7-1272-4a8c-bf06-da6010247a7c"),
                        new Guid("1696b27d-8452-458a-994b-fdeef9cff690"), "X7", 2000.0, 0,
                    },
                });

            migrationBuilder.InsertData(
                "StoreCategoryRelation",
                new[] { "StoreDbModelId", "CategoryDbModelId" },
                new object[,]
                {
                    {
                        new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"),
                        new Guid("080917f2-e2fa-4581-a7c2-743b259852ef"),
                    },
                    { new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"), new Guid("1696b27d-8452-458a-994b-fdeef9cff690") },
                });

            migrationBuilder.InsertData(
                "StoreProductRelation",
                new[] { "StoreDbModelId", "ProductDbModelId" },
                new object[,]
                {
                    {
                        new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"),
                        new Guid("e15a240e-bced-4458-a6b6-347e06abe88c"),
                    },
                    {
                        new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"),
                        new Guid("76946bca-9fb0-4eff-8b46-a4949e256a1c"),
                    },
                    {
                        new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"),
                        new Guid("6e367c9e-2c95-4552-9599-820a15c29e7a"),
                    },
                    {
                        new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"),
                        new Guid("d713dc3b-92a3-45cc-befb-f70a82c0323c"),
                    },
                    {
                        new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"),
                        new Guid("004f7b54-d482-4418-b6f4-ded434ee2d76"),
                    },
                    {
                        new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"),
                        new Guid("d10d49a3-dbfa-4946-a3bd-66f93b62e5e1"),
                    },
                    {
                        new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"),
                        new Guid("5445426b-308e-4344-9a73-ca39cb5bc674"),
                    },
                    {
                        new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"),
                        new Guid("611c8fab-b1c5-485e-a8ab-d078deb9f2d8"),
                    },
                    {
                        new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"),
                        new Guid("c8baed60-1600-483d-9d3e-6004cabc19c1"),
                    },
                    {
                        new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"),
                        new Guid("437de9d5-f77e-43a3-9b1c-490e78a782a7"),
                    },
                    { new Guid("a46c798b-2b14-4a81-bb1e-bba9c3cad640"), new Guid("54da41e7-1272-4a8c-bf06-da6010247a7c") },
                });

            migrationBuilder.CreateIndex(
                "IX_Categories_StoreDbModelId",
                "Categories",
                "StoreDbModelId");

            migrationBuilder.CreateIndex(
                "IX_Products_CategoryId",
                "Products",
                "CategoryId");

            migrationBuilder.CreateIndex(
                "IX_StoreCategoryRelation_CategoryDbModelId",
                "StoreCategoryRelation",
                "CategoryDbModelId");

            migrationBuilder.CreateIndex(
                "IX_StoreProductRelation_ProductDbModelId",
                "StoreProductRelation",
                "ProductDbModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Books");

            migrationBuilder.DropTable(
                "Clients");

            migrationBuilder.DropTable(
                "Purchases");

            migrationBuilder.DropTable(
                "Roles");

            migrationBuilder.DropTable(
                "StoreCategoryRelation");

            migrationBuilder.DropTable(
                "StoreProductRelation");

            migrationBuilder.DropTable(
                "UserRoles");

            migrationBuilder.DropTable(
                "Users");

            migrationBuilder.DropTable(
                "Products");

            migrationBuilder.DropTable(
                "Categories");

            migrationBuilder.DropTable(
                "Stores");
        }
    }
}