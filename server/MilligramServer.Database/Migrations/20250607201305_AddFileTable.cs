using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilligramServer.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddFileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Messages_FilePath",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Messages");

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "Messages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsImage = table.Column<bool>(type: "bit", nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.CheckConstraint("CK_Files_Extension", "LEN(Extension) > 0");
                    table.CheckConstraint("CK_Files_Name", "LEN(Name) > 0");
                    table.CheckConstraint("CK_Files_SizeBytes", "SizeBytes > 0 AND SizeBytes <= 10485760");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_FileId",
                table: "Messages",
                column: "FileId",
                unique: true,
                filter: "[FileId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Files_FileId",
                table: "Messages",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Files_FileId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Messages_FileId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Messages_FilePath",
                table: "Messages",
                sql: "FilePath IS NULL OR LEN(FilePath) > 0");
        }
    }
}
