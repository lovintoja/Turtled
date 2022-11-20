using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurtledAuth.Data.Migrations.UserStore
{
    /// <inheritdoc />
    public partial class RefreshIdMovedToUserToken3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshId",
                schema: "Identity",
                table: "UserTokens",
                newName: "RefreshToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                schema: "Identity",
                table: "UserTokens",
                newName: "RefreshId");
        }
    }
}
