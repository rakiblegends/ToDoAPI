using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoAPI.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TodoTasks_DueDate",
                table: "TodoTasks",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_TodoTasks_Priority",
                table: "TodoTasks",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_TodoTasks_Title",
                table: "TodoTasks",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoTasks_DueDate",
                table: "TodoTasks");

            migrationBuilder.DropIndex(
                name: "IX_TodoTasks_Priority",
                table: "TodoTasks");

            migrationBuilder.DropIndex(
                name: "IX_TodoTasks_Title",
                table: "TodoTasks");
        }
    }
}
