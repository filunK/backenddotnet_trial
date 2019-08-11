using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace backenddotnet_trial.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "BK_DOTNET");

            migrationBuilder.CreateTable(
                name: "T_USER",
                schema: "BK_DOTNET",
                columns: table => new
                {
                    USER_ID = table.Column<string>(nullable: false),
                    SALT = table.Column<string>(nullable: false),
                    HASH = table.Column<string>(nullable: false),
                    ITERATION = table.Column<int>(nullable: false),
                    ACCOUNT_CONFIRMED = table.Column<bool>(nullable: false),
                    CREATION_ID = table.Column<string>(maxLength: 30, nullable: false),
                    CREATION_PG = table.Column<string>(maxLength: 30, nullable: false),
                    CREATION_DATE = table.Column<DateTime>(nullable: false),
                    UPDATE_ID = table.Column<string>(maxLength: 30, nullable: false),
                    UPDATE_DATE = table.Column<DateTime>(nullable: false),
                    DEL_FLAG = table.Column<bool>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_USER", x => x.USER_ID);
                });

            migrationBuilder.CreateTable(
                name: "WK_ACCOUNT_CONFIRM",
                schema: "BK_DOTNET",
                columns: table => new
                {
                    CONFIRM_URI = table.Column<string>(maxLength: 40, nullable: false),
                    USER_ID = table.Column<string>(nullable: true),
                    EXPIRE_LIMIT = table.Column<DateTime>(nullable: false),
                    CREATION_ID = table.Column<string>(maxLength: 30, nullable: false),
                    CREATION_PG = table.Column<string>(maxLength: 30, nullable: false),
                    CREATION_DATE = table.Column<DateTime>(nullable: false),
                    UPDATE_ID = table.Column<string>(maxLength: 30, nullable: false),
                    UPDATE_DATE = table.Column<DateTime>(nullable: false),
                    DEL_FLAG = table.Column<bool>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WK_ACCOUNT_CONFIRM", x => x.CONFIRM_URI);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_USER",
                schema: "BK_DOTNET");

            migrationBuilder.DropTable(
                name: "WK_ACCOUNT_CONFIRM",
                schema: "BK_DOTNET");
        }
    }
}
