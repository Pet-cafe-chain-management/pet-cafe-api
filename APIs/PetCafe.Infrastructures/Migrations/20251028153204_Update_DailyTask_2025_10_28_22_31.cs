using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_DailyTask_2025_10_28_22_31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_daily_tasks_tasks_task_id",
                table: "daily_tasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "task_id",
                table: "daily_tasks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "daily_tasks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "priority",
                table: "daily_tasks",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "daily_tasks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "f_k_daily_tasks_tasks_task_id",
                table: "daily_tasks",
                column: "task_id",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_daily_tasks_tasks_task_id",
                table: "daily_tasks");

            migrationBuilder.DropColumn(
                name: "description",
                table: "daily_tasks");

            migrationBuilder.DropColumn(
                name: "priority",
                table: "daily_tasks");

            migrationBuilder.DropColumn(
                name: "title",
                table: "daily_tasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "task_id",
                table: "daily_tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "f_k_daily_tasks_tasks_task_id",
                table: "daily_tasks",
                column: "task_id",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
