using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_Daily_Tasks_2025_11_15_12_47 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "vaccination_schedule_id",
                table: "daily_tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_daily_tasks_vaccination_schedule_id",
                table: "daily_tasks",
                column: "vaccination_schedule_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_daily_tasks_vaccination_schedules_vaccination_schedule_id",
                table: "daily_tasks",
                column: "vaccination_schedule_id",
                principalTable: "vaccination_schedules",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_daily_tasks_vaccination_schedules_vaccination_schedule_id",
                table: "daily_tasks");

            migrationBuilder.DropIndex(
                name: "i_x_daily_tasks_vaccination_schedule_id",
                table: "daily_tasks");

            migrationBuilder.DropColumn(
                name: "vaccination_schedule_id",
                table: "daily_tasks");
        }
    }
}
