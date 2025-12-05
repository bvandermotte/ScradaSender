using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScradaSender.Migrations
{
    /// <inheritdoc />
    public partial class addedPeppolIdMadeErrorNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "FileStatusses",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "PeppolId",
                table: "FileStatusses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeppolId",
                table: "FileStatusses");

            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "FileStatusses",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
