using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ajedrez.Infrastructure.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class EsquemaInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntradasLibroApertura",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClavePosicion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Origen = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Destino = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    PromocionA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VecesJugado = table.Column<int>(type: "int", nullable: false),
                    Victorias = table.Column<int>(type: "int", nullable: false),
                    Derrotas = table.Column<int>(type: "int", nullable: false),
                    Tablas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntradasLibroApertura", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Partidas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TurnoActual = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlancoEnroqueCorto = table.Column<bool>(type: "bit", nullable: false),
                    BlancoEnroqueLargo = table.Column<bool>(type: "bit", nullable: false),
                    NegroEnroqueCorto = table.Column<bool>(type: "bit", nullable: false),
                    NegroEnroqueLargo = table.Column<bool>(type: "bit", nullable: false),
                    CasillaAlPasoFila = table.Column<int>(type: "int", nullable: true),
                    CasillaAlPasoColumna = table.Column<int>(type: "int", nullable: true),
                    ContadorMediosMovimientosSinCaptura = table.Column<int>(type: "int", nullable: false),
                    Ganador = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MotivoFinalizacion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClavesPosicion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartidaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Clave = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClavesPosicion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClavesPosicion_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartidaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    OrigenFila = table.Column<int>(type: "int", nullable: false),
                    OrigenColumna = table.Column<int>(type: "int", nullable: false),
                    DestinoFila = table.Column<int>(type: "int", nullable: false),
                    DestinoColumna = table.Column<int>(type: "int", nullable: false),
                    PromocionA = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EsEnroqueCorto = table.Column<bool>(type: "bit", nullable: false),
                    EsEnroqueLargo = table.Column<bool>(type: "bit", nullable: false),
                    EsCapturaAlPaso = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movimientos_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PiezasTablero",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartidaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fila = table.Column<int>(type: "int", nullable: false),
                    Columna = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PiezasTablero", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PiezasTablero_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClavesPosicion_PartidaId_Orden",
                table: "ClavesPosicion",
                columns: new[] { "PartidaId", "Orden" });

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_PartidaId_Orden",
                table: "Movimientos",
                columns: new[] { "PartidaId", "Orden" });

            migrationBuilder.CreateIndex(
                name: "IX_PiezasTablero_PartidaId",
                table: "PiezasTablero",
                column: "PartidaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClavesPosicion");

            migrationBuilder.DropTable(
                name: "EntradasLibroApertura");

            migrationBuilder.DropTable(
                name: "Movimientos");

            migrationBuilder.DropTable(
                name: "PiezasTablero");

            migrationBuilder.DropTable(
                name: "Partidas");
        }
    }
}
