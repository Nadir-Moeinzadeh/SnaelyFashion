using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnaelyFashion_WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class profilepic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProfilePicture_ApplicationUserId",
                table: "ProfilePicture");

            migrationBuilder.DropColumn(
                name: "ProfilePictureURL",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePicture_ApplicationUserId",
                table: "ProfilePicture",
                column: "ApplicationUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProfilePicture_ApplicationUserId",
                table: "ProfilePicture");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureURL",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePicture_ApplicationUserId",
                table: "ProfilePicture",
                column: "ApplicationUserId");
        }
    }
}
