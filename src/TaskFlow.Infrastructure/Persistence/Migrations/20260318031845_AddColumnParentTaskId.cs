using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnParentTaskId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdTaskPai",
                table: "Tarefa",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tarefa_IdTaskPai",
                table: "Tarefa",
                column: "IdTaskPai");

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefa_Tarefa_IdTaskPai",
                table: "Tarefa",
                column: "IdTaskPai",
                principalTable: "Tarefa",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarefa_Tarefa_IdTaskPai",
                table: "Tarefa");

            migrationBuilder.DropIndex(
                name: "IX_Tarefa_IdTaskPai",
                table: "Tarefa");

            migrationBuilder.DropColumn(
                name: "IdTaskPai",
                table: "Tarefa");
        }
    }
}
