atusing Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpdeskRepository.Migrations
{
    /// <inheritdoc />
    public partial class AttachmentAddToNotice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notices_AttachmentId",
                table: "Notices",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notices_ModifiedById",
                table: "Notices",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Notices_AspNetUsers_ModifiedById",
                table: "Notices",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notices_Attachments_AttachmentId",
                table: "Notices",
                column: "AttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notices_AspNetUsers_ModifiedById",
                table: "Notices");

            migrationBuilder.DropForeignKey(
                name: "FK_Notices_Attachments_AttachmentId",
                table: "Notices");

            migrationBuilder.DropIndex(
                name: "IX_Notices_AttachmentId",
                table: "Notices");

            migrationBuilder.DropIndex(
                name: "IX_Notices_ModifiedById",
                table: "Notices");
        }
    }
}
