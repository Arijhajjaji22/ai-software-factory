using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiSoftwareFactory.Agents.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdeeMetier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EtapeActuelle = table.Column<int>(type: "int", nullable: false),
                    StatutEtapeActuelle = table.Column<int>(type: "int", nullable: false),
                    ResultatAnalyse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultatArchitecture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultatDevBackend = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultatDevFrontend = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultatDevOps = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoriqueValidations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Etape = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Commentaire = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAction = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriqueValidations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriqueValidations_Projets_ProjetId",
                        column: x => x.ProjetId,
                        principalTable: "Projets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueValidations_ProjetId",
                table: "HistoriqueValidations",
                column: "ProjetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoriqueValidations");

            migrationBuilder.DropTable(
                name: "Projets");
        }
    }
}
