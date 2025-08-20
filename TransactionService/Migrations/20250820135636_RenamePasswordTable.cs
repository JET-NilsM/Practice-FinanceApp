using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionService.Migrations
{
    /// <inheritdoc />
    public partial class RenamePasswordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EncrypedPasswords",
                table: "EncrypedPasswords");

            migrationBuilder.RenameTable(
                name: "EncrypedPasswords",
                newName: "HashedPasswords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HashedPasswords",
                table: "HashedPasswords",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HashedPasswords",
                table: "HashedPasswords");

            migrationBuilder.RenameTable(
                name: "HashedPasswords",
                newName: "EncrypedPasswords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EncrypedPasswords",
                table: "EncrypedPasswords",
                column: "Id");
        }
    }
}
