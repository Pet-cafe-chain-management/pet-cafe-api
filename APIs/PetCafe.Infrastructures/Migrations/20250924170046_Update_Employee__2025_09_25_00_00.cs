using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_Employee__2025_09_25_00_00 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_employees_employees_leader_id",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "i_x_employees_leader_id",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "hire_date",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "leader_id",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "position",
                table: "employees");

            migrationBuilder.AddColumn<string>(
                name: "skills",
                table: "employees",
                type: "json",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "skills",
                table: "employees");

            migrationBuilder.AddColumn<DateTime>(
                name: "hire_date",
                table: "employees",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "leader_id",
                table: "employees",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "position",
                table: "employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_employees_leader_id",
                table: "employees",
                column: "leader_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_employees_employees_leader_id",
                table: "employees",
                column: "leader_id",
                principalTable: "employees",
                principalColumn: "id");
        }
    }
}
