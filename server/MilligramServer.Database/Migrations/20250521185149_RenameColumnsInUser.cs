using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilligramServer.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnsInUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Users_OwnerUserId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersChats_Chats_ChatId",
                table: "UsersChats");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersChats_Users_UserId",
                table: "UsersChats");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Roles_RoleId",
                table: "UsersRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Users_UserId",
                table: "UsersRoles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Login",
                table: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_NormalizedLogin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Login",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "NormalizedLogin",
                table: "Users",
                newName: "NormalizedName");

            migrationBuilder.RenameIndex(
                name: "IX_Users_NormalizedLogin",
                table: "Users",
                newName: "IX_Users_NormalizedName");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Nickname",
                table: "Users",
                sql: "LEN(Nickname) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_NormalizedName",
                table: "Users",
                sql: "LEN(NormalizedName) > 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Users_OwnerUserId",
                table: "Contacts",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersChats_Chats_ChatId",
                table: "UsersChats",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersChats_Users_UserId",
                table: "UsersChats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersRoles_Roles_RoleId",
                table: "UsersRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersRoles_Users_UserId",
                table: "UsersRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Users_OwnerUserId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersChats_Chats_ChatId",
                table: "UsersChats");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersChats_Users_UserId",
                table: "UsersChats");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Roles_RoleId",
                table: "UsersRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Users_UserId",
                table: "UsersRoles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Nickname",
                table: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_NormalizedName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "NormalizedName",
                table: "Users",
                newName: "NormalizedLogin");

            migrationBuilder.RenameIndex(
                name: "IX_Users_NormalizedName",
                table: "Users",
                newName: "IX_Users_NormalizedLogin");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Login",
                table: "Users",
                sql: "LEN(Login) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_NormalizedLogin",
                table: "Users",
                sql: "LEN(NormalizedLogin) > 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Users_OwnerUserId",
                table: "Contacts",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersChats_Chats_ChatId",
                table: "UsersChats",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersChats_Users_UserId",
                table: "UsersChats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersRoles_Roles_RoleId",
                table: "UsersRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersRoles_Users_UserId",
                table: "UsersRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
