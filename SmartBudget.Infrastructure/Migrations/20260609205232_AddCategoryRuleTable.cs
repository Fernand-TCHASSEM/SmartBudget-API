using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartBudget.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryRuleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_refresh_tokens_token",
                table: "refresh_tokens");

            migrationBuilder.AlterColumn<string>(
                name: "color",
                table: "categories",
                type: "varchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "#6B7280",
                oldClrType: typeof(string),
                oldType: "varchar(7)",
                oldMaxLength: 7)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "category_rules",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    keyword = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_regex = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    priority = table.Column<int>(type: "int", maxLength: 100, nullable: false, defaultValue: 100),
                    source = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    user_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    category_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category_rules", x => x.id);
                    table.ForeignKey(
                        name: "fk_category_rules_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_category_rules_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_category_rules_category_id",
                table: "category_rules",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_category_rules_deleted_at",
                table: "category_rules",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_category_rules_priority",
                table: "category_rules",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "ix_category_rules_user_id",
                table: "category_rules",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "category_rules");

            migrationBuilder.AlterColumn<string>(
                name: "color",
                table: "categories",
                type: "varchar(7)",
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(7)",
                oldMaxLength: 7,
                oldDefaultValue: "#6B7280")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token",
                table: "refresh_tokens",
                column: "token");
        }
    }
}
