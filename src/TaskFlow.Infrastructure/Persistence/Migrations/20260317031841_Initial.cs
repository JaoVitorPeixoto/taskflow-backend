using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Senha = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    DataCriado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlterado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lista",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataCriado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlterado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lista", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lista_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tarefa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdLista = table.Column<Guid>(type: "uuid", nullable: true),
                    IdUsuario = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Completada = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataCompletada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Prioridade = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    DataAgendamento = table.Column<DateOnly>(type: "date", nullable: true),
                    HoraAgendamento = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Recorrencia = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NotificarAgendamento = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlterado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarefa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tarefa_Lista_IdLista",
                        column: x => x.IdLista,
                        principalTable: "Lista",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tarefa_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lista_IdUsuario",
                table: "Lista",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefa_IdLista",
                table: "Tarefa",
                column: "IdLista");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefa_IdUsuario",
                table: "Tarefa",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tarefa");

            migrationBuilder.DropTable(
                name: "Lista");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
