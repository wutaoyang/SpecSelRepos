using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SpecSelRepos.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpecSelResult",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AreaPrecisionThresholdY = table.Column<decimal>(nullable: false),
                    DataSet = table.Column<string>(maxLength: 60, nullable: false),
                    NumResources = table.Column<int>(nullable: false),
                    NumSpecies = table.Column<int>(nullable: false),
                    Option = table.Column<string>(maxLength: 3, nullable: false),
                    Output = table.Column<string>(nullable: false),
                    SdThresholdX = table.Column<decimal>(nullable: false),
                    SpeciesThresholdM = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecSelResult", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecSelResult");
        }
    }
}
