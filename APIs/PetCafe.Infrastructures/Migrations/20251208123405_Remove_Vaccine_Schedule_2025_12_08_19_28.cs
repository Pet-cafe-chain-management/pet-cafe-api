using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Vaccine_Schedule_2025_12_08_19_28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_vaccination_records_vaccine_types_vaccine_type_id",
                table: "vaccination_records");

            migrationBuilder.DropForeignKey(
                name: "f_k_vaccination_schedules_vaccine_types_vaccine_type_id",
                table: "vaccination_schedules");

            migrationBuilder.DropTable(
                name: "vaccine_types");

            migrationBuilder.DropIndex(
                name: "i_x_vaccination_schedules_vaccine_type_id",
                table: "vaccination_schedules");

            migrationBuilder.DropIndex(
                name: "i_x_vaccination_records_pet_id_vaccine_type_id_vaccination_date",
                table: "vaccination_records");

            migrationBuilder.DropIndex(
                name: "i_x_vaccination_records_vaccine_type_id",
                table: "vaccination_records");

            migrationBuilder.DropColumn(
                name: "vaccine_type_id",
                table: "vaccination_schedules");

            migrationBuilder.DropColumn(
                name: "vaccine_type_id",
                table: "vaccination_records");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "vaccination_records",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_records_pet_id_name_vaccination_date",
                table: "vaccination_records",
                columns: new[] { "pet_id", "name", "vaccination_date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_vaccination_records_pet_id_name_vaccination_date",
                table: "vaccination_records");

            migrationBuilder.DropColumn(
                name: "name",
                table: "vaccination_records");

            migrationBuilder.AddColumn<Guid>(
                name: "vaccine_type_id",
                table: "vaccination_schedules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "vaccine_type_id",
                table: "vaccination_records",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "vaccine_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    species_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    interval_months = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    required_doses = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccine_types", x => x.id);
                    table.ForeignKey(
                        name: "f_k_vaccine_types_pet_species_species_id",
                        column: x => x.species_id,
                        principalTable: "pet_species",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_schedules_vaccine_type_id",
                table: "vaccination_schedules",
                column: "vaccine_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_records_pet_id_vaccine_type_id_vaccination_date",
                table: "vaccination_records",
                columns: new[] { "pet_id", "vaccine_type_id", "vaccination_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_records_vaccine_type_id",
                table: "vaccination_records",
                column: "vaccine_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_vaccine_types_name",
                table: "vaccine_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_vaccine_types_species_id",
                table: "vaccine_types",
                column: "species_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_vaccination_records_vaccine_types_vaccine_type_id",
                table: "vaccination_records",
                column: "vaccine_type_id",
                principalTable: "vaccine_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_vaccination_schedules_vaccine_types_vaccine_type_id",
                table: "vaccination_schedules",
                column: "vaccine_type_id",
                principalTable: "vaccine_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
