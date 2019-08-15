using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace backenddotnet_trial.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "bk_dotnet");

            migrationBuilder.CreateTable(
                name: "t_user",
                schema: "bk_dotnet",
                columns: table => new
                {
                    user_id = table.Column<string>(maxLength: 100, nullable: false),
                    mail_address = table.Column<string>(nullable: false),
                    salt = table.Column<string>(nullable: false),
                    hash = table.Column<string>(nullable: false),
                    iteration = table.Column<int>(nullable: false),
                    account_confirmed = table.Column<bool>(nullable: false),
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
                    table.PrimaryKey("PK_t_user", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "wk_account_confirm",
                schema: "bk_dotnet",
                columns: table => new
                {
                    confirm_uri = table.Column<string>(maxLength: 40, nullable: false),
                    user_id = table.Column<string>(maxLength: 100, nullable: true),
                    expire_limit = table.Column<DateTime>(nullable: false),
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
                    table.PrimaryKey("PK_wk_account_confirm", x => x.confirm_uri);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_user",
                schema: "bk_dotnet");

            migrationBuilder.DropTable(
                name: "wk_account_confirm",
                schema: "bk_dotnet");
        }
    }
}
