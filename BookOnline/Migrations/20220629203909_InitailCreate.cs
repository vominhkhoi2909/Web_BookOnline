using Microsoft.EntityFrameworkCore.Migrations;

namespace BookOnline.Migrations
{
    public partial class InitailCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Account_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account_Name = table.Column<string>(type: "varchar(20)", nullable: true),
                    Account_Password = table.Column<string>(type: "varchar(MAX)", nullable: true),
                    Account_ConfirmPassword = table.Column<string>(type: "varchar(MAX)", nullable: true),
                    Account_Email = table.Column<string>(type: "varchar(50)", nullable: true),
                    Account_Phone = table.Column<int>(type: "int", nullable: false),
                    Account_Type = table.Column<int>(type: "int", nullable: false),
                    Account_Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Account_Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Category_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category_Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Category_DataName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Category_Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Category_Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Book_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Book_Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Book_Author = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Book_Content = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    Book_ShortContent = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    Book_Category = table.Column<int>(type: "int", nullable: false),
                    Category_Id = table.Column<int>(type: "int", nullable: true),
                    Book_Img = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    Book_Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Book_NXB = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Book_View = table.Column<int>(type: "int", nullable: false),
                    Book_Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Book_Id);
                    table.ForeignKey(
                        name: "FK_Books_Categories_Category_Id",
                        column: x => x.Category_Id,
                        principalTable: "Categories",
                        principalColumn: "Category_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_Category_Id",
                table: "Books",
                column: "Category_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
