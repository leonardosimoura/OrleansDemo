using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSOrleansDemo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgreementStates",
                columns: table => new
                {
                    AgreementId = table.Column<string>(type: "text", nullable: false),
                    PdfFileLocation = table.Column<string>(type: "text", nullable: true),
                    SignerId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgreementStates", x => x.AgreementId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgreementStates");
        }
    }
}
