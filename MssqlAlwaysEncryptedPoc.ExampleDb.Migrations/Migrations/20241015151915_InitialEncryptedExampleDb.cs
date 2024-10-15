using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MssqlAlwaysEncryptedPoc.ExampleDb.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialEncryptedExampleDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EncryptedExamples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncryptedExamples", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncryptedExamples");
        }
    }
}
