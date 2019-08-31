using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace backenddotnet_trial.Migrations
{
    public partial class initial_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_book",
                schema: "bk_dotnet",
                columns: table => new
                {
                    user_id = table.Column<string>(maxLength: 100, nullable: false),
                    isbn_13 = table.Column<string>(maxLength: 13, nullable: false),
                    title = table.Column<string>(nullable: false),
                    author = table.Column<string>(nullable: false),
                    page_count = table.Column<int>(nullable: false),
                    purchase_date = table.Column<DateTime>(nullable: false),
                    comment = table.Column<string>(nullable: true),
                    creation_id = table.Column<string>(maxLength: 30, nullable: false),
                    creation_pg = table.Column<string>(maxLength: 30, nullable: false),
                    creation_date = table.Column<DateTime>(nullable: false),
                    update_id = table.Column<string>(maxLength: 30, nullable: false),
                    update_pg = table.Column<string>(maxLength: 30, nullable: false),
                    update_date = table.Column<DateTime>(nullable: false),
                    del_flg = table.Column<bool>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_book", x => new { x.user_id, x.isbn_13 });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_book",
                schema: "bk_dotnet");
        }
    }
}
