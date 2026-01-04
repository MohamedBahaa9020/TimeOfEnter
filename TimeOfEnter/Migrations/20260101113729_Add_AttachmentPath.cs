using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeOfEnter.Migrations
{
    /// <inheritdoc />
    public partial class Add_AttachmentPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentPath",
                table: "AspNetUsers");
        }
    }
}
