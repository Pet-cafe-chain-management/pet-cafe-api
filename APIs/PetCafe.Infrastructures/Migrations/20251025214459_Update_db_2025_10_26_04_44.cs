using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_db_2025_10_26_04_44 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_areas_work_types_work_type_id",
                table: "areas");

            migrationBuilder.DropForeignKey(
                name: "f_k_services_work_types_work_type_id",
                table: "services");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_pet_groups_pet_gourp_id",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_services_service_id",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_tasks_areas_area_id",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "f_k_tasks_pet_groups_pet_group_id",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "f_k_tasks_pets_pet_id",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "f_k_tasks_teams_team_id",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "f_k_tasks_work_shifts_work_shift_id",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "f_k_teams_areas_area_id",
                table: "teams");

            migrationBuilder.DropForeignKey(
                name: "f_k_teams_work_types_work_type_id",
                table: "teams");

            migrationBuilder.DropTable(
                name: "service_pet_groups");

            migrationBuilder.DropTable(
                name: "task_assignments");

            migrationBuilder.DropIndex(
                name: "i_x_teams_area_id",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "i_x_teams_work_type_id",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "i_x_tasks_area_id",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "i_x_tasks_due_date",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "i_x_tasks_pet_group_id",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "i_x_tasks_pet_id",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "i_x_tasks_team_id_status",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "i_x_tasks_work_shift_id",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "i_x_services_work_type_id",
                table: "services");

            migrationBuilder.DropIndex(
                name: "i_x_areas_work_type_id",
                table: "areas");

            migrationBuilder.DropColumn(
                name: "area_id",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "work_type_id",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "actual_hours",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "area_id",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "due_date",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "pet_group_id",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "pet_id",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "applicable_days",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "status",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "work_type_id",
                table: "areas");

            migrationBuilder.RenameColumn(
                name: "work_shift_id",
                table: "tasks",
                newName: "service_id");

            migrationBuilder.RenameColumn(
                name: "team_id",
                table: "tasks",
                newName: "work_type_id");

            migrationBuilder.RenameColumn(
                name: "pet_gourp_id",
                table: "slots",
                newName: "pet_group_id");

            migrationBuilder.RenameIndex(
                name: "i_x_slots_pet_gourp_id",
                table: "slots",
                newName: "i_x_slots_pet_group_id");

            migrationBuilder.RenameColumn(
                name: "work_type_id",
                table: "services",
                newName: "task_id");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "tasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "tasks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_recurring",
                table: "tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "service_id",
                table: "slots",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "is_active",
                table: "slots",
                type: "text",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<Guid>(
                name: "pet_group_id",
                table: "slots",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "area_id1",
                table: "slots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "day_of_week",
                table: "slots",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "pet_group_id1",
                table: "slots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "pet_id",
                table: "slots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "pet_id1",
                table: "slots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "service_id1",
                table: "slots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "task_id",
                table: "slots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "task_id1",
                table: "slots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "team_id",
                table: "slots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "team_id1",
                table: "slots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "assigned_date",
                table: "daily_tasks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                table: "daily_tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "daily_tasks",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "slot_id",
                table: "daily_tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "daily_tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "area_work_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    area_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_area_work_types", x => x.id);
                    table.ForeignKey(
                        name: "f_k_area_work_types_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_area_work_types_work_types_work_type_id",
                        column: x => x.work_type_id,
                        principalTable: "work_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team_work_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_work_types", x => x.id);
                    table.ForeignKey(
                        name: "f_k_team_work_types_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_team_work_types_work_types_work_type_id",
                        column: x => x.work_type_id,
                        principalTable: "work_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_status",
                table: "tasks",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_work_type_id",
                table: "tasks",
                column: "work_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_area_id1",
                table: "slots",
                column: "area_id1");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_pet_group_id1",
                table: "slots",
                column: "pet_group_id1");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_pet_id",
                table: "slots",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_pet_id1",
                table: "slots",
                column: "pet_id1");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_service_id1",
                table: "slots",
                column: "service_id1");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_task_id",
                table: "slots",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_task_id1",
                table: "slots",
                column: "task_id1");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_team_id",
                table: "slots",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_team_id1",
                table: "slots",
                column: "team_id1");

            migrationBuilder.CreateIndex(
                name: "i_x_services_task_id",
                table: "services",
                column: "task_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_daily_tasks_slot_id",
                table: "daily_tasks",
                column: "slot_id");

            migrationBuilder.CreateIndex(
                name: "i_x_area_work_types_area_id",
                table: "area_work_types",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "i_x_area_work_types_work_type_id",
                table: "area_work_types",
                column: "work_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_team_work_types_team_id",
                table: "team_work_types",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "i_x_team_work_types_work_type_id",
                table: "team_work_types",
                column: "work_type_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_daily_tasks_slots_slot_id",
                table: "daily_tasks",
                column: "slot_id",
                principalTable: "slots",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_services_tasks_task_id",
                table: "services",
                column: "task_id",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_areas_area_id1",
                table: "slots",
                column: "area_id1",
                principalTable: "areas",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_pet_groups_pet_group_id",
                table: "slots",
                column: "pet_group_id",
                principalTable: "pet_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_pet_groups_pet_group_id1",
                table: "slots",
                column: "pet_group_id1",
                principalTable: "pet_groups",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_pets_pet_id",
                table: "slots",
                column: "pet_id",
                principalTable: "pets",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_pets_pet_id1",
                table: "slots",
                column: "pet_id1",
                principalTable: "pets",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_services_service_id",
                table: "slots",
                column: "service_id",
                principalTable: "services",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_services_service_id1",
                table: "slots",
                column: "service_id1",
                principalTable: "services",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_tasks_task_id",
                table: "slots",
                column: "task_id",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_tasks_task_id1",
                table: "slots",
                column: "task_id1",
                principalTable: "tasks",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_teams_team_id",
                table: "slots",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_teams_team_id1",
                table: "slots",
                column: "team_id1",
                principalTable: "teams",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_tasks_work_types_work_type_id",
                table: "tasks",
                column: "work_type_id",
                principalTable: "work_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_daily_tasks_slots_slot_id",
                table: "daily_tasks");

            migrationBuilder.DropForeignKey(
                name: "f_k_services_tasks_task_id",
                table: "services");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_areas_area_id1",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_pet_groups_pet_group_id",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_pet_groups_pet_group_id1",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_pets_pet_id",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_pets_pet_id1",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_services_service_id",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_services_service_id1",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_tasks_task_id",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_tasks_task_id1",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_teams_team_id",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_teams_team_id1",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_tasks_work_types_work_type_id",
                table: "tasks");

            migrationBuilder.DropTable(
                name: "area_work_types");

            migrationBuilder.DropTable(
                name: "team_work_types");

            migrationBuilder.DropIndex(
                name: "i_x_tasks_status",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "i_x_tasks_work_type_id",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "i_x_slots_area_id1",
                table: "slots");

            migrationBuilder.DropIndex(
                name: "i_x_slots_pet_group_id1",
                table: "slots");

            migrationBuilder.DropIndex(
                name: "i_x_slots_pet_id",
                table: "slots");

            migrationBuilder.DropIndex(
                name: "i_x_slots_pet_id1",
                table: "slots");

            migrationBuilder.DropIndex(
                name: "i_x_slots_service_id1",
                table: "slots");

            migrationBuilder.DropIndex(
                name: "i_x_slots_task_id",
                table: "slots");

            migrationBuilder.DropIndex(
                name: "i_x_slots_task_id1",
                table: "slots");

            migrationBuilder.DropIndex(
                name: "i_x_slots_team_id",
                table: "slots");

            migrationBuilder.DropIndex(
                name: "i_x_slots_team_id1",
                table: "slots");

            migrationBuilder.DropIndex(
                name: "i_x_services_task_id",
                table: "services");

            migrationBuilder.DropIndex(
                name: "i_x_daily_tasks_slot_id",
                table: "daily_tasks");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "is_recurring",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "area_id1",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "day_of_week",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "pet_group_id1",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "pet_id",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "pet_id1",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "service_id1",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "task_id",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "task_id1",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "team_id1",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "assigned_date",
                table: "daily_tasks");

            migrationBuilder.DropColumn(
                name: "completion_date",
                table: "daily_tasks");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "daily_tasks");

            migrationBuilder.DropColumn(
                name: "slot_id",
                table: "daily_tasks");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "daily_tasks");

            migrationBuilder.RenameColumn(
                name: "work_type_id",
                table: "tasks",
                newName: "team_id");

            migrationBuilder.RenameColumn(
                name: "service_id",
                table: "tasks",
                newName: "work_shift_id");

            migrationBuilder.RenameColumn(
                name: "pet_group_id",
                table: "slots",
                newName: "pet_gourp_id");

            migrationBuilder.RenameIndex(
                name: "i_x_slots_pet_group_id",
                table: "slots",
                newName: "i_x_slots_pet_gourp_id");

            migrationBuilder.RenameColumn(
                name: "task_id",
                table: "services",
                newName: "work_type_id");

            migrationBuilder.AddColumn<Guid>(
                name: "area_id",
                table: "teams",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "work_type_id",
                table: "teams",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "tasks",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "actual_hours",
                table: "tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "area_id",
                table: "tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "due_date",
                table: "tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "pet_group_id",
                table: "tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "pet_id",
                table: "tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "service_id",
                table: "slots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "slots",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "pet_gourp_id",
                table: "slots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "applicable_days",
                table: "slots",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "slots",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "work_type_id",
                table: "areas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "service_pet_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_pet_groups", x => x.id);
                    table.ForeignKey(
                        name: "f_k_service_pet_groups_pet_groups_pet_group_id",
                        column: x => x.pet_group_id,
                        principalTable: "pet_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_service_pet_groups_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "task_assignments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completion_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_assignments", x => x.id);
                    table.ForeignKey(
                        name: "f_k_task_assignments_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_task_assignments_tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_teams_area_id",
                table: "teams",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "i_x_teams_work_type_id",
                table: "teams",
                column: "work_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_area_id",
                table: "tasks",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_due_date",
                table: "tasks",
                column: "due_date");

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_pet_group_id",
                table: "tasks",
                column: "pet_group_id");

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_pet_id",
                table: "tasks",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_team_id_status",
                table: "tasks",
                columns: new[] { "team_id", "status" });

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_work_shift_id",
                table: "tasks",
                column: "work_shift_id");

            migrationBuilder.CreateIndex(
                name: "i_x_services_work_type_id",
                table: "services",
                column: "work_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_areas_work_type_id",
                table: "areas",
                column: "work_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_service_pet_groups_pet_group_id",
                table: "service_pet_groups",
                column: "pet_group_id");

            migrationBuilder.CreateIndex(
                name: "i_x_service_pet_groups_service_id",
                table: "service_pet_groups",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "i_x_task_assignments_employee_id_status",
                table: "task_assignments",
                columns: new[] { "employee_id", "status" });

            migrationBuilder.CreateIndex(
                name: "i_x_task_assignments_task_id_employee_id",
                table: "task_assignments",
                columns: new[] { "task_id", "employee_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "f_k_areas_work_types_work_type_id",
                table: "areas",
                column: "work_type_id",
                principalTable: "work_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_services_work_types_work_type_id",
                table: "services",
                column: "work_type_id",
                principalTable: "work_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_pet_groups_pet_gourp_id",
                table: "slots",
                column: "pet_gourp_id",
                principalTable: "pet_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_services_service_id",
                table: "slots",
                column: "service_id",
                principalTable: "services",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_tasks_areas_area_id",
                table: "tasks",
                column: "area_id",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_tasks_pet_groups_pet_group_id",
                table: "tasks",
                column: "pet_group_id",
                principalTable: "pet_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_tasks_pets_pet_id",
                table: "tasks",
                column: "pet_id",
                principalTable: "pets",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_tasks_teams_team_id",
                table: "tasks",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_tasks_work_shifts_work_shift_id",
                table: "tasks",
                column: "work_shift_id",
                principalTable: "work_shifts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_teams_areas_area_id",
                table: "teams",
                column: "area_id",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_teams_work_types_work_type_id",
                table: "teams",
                column: "work_type_id",
                principalTable: "work_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
