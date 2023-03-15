using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoist.Data.Migrations
{
    /// <inheritdoc />
    public partial class _006_CompanyDescriptionLengthShortened : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Companies",
                type: "character varying(400)",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40000)",
                oldMaxLength: 40000,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Companies",
                type: "character varying(40000)",
                maxLength: 40000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(400)",
                oldMaxLength: 400,
                oldNullable: true);
        }
    }
}
