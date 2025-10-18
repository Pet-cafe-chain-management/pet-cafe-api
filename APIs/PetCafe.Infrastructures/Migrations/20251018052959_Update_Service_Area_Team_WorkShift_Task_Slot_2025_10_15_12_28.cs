using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_Service_Area_Team_WorkShift_Task_Slot_2025_10_15_12_28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_customer_bookings_pet_groups_pet_group_id",
                table: "customer_bookings");

            migrationBuilder.DropForeignKey(
                name: "f_k_employees_areas_area_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "f_k_pet_groups_areas_area_id",
                table: "pet_groups");

            migrationBuilder.DropForeignKey(
                name: "f_k_slots_teams_team_id",
                table: "slots");

            migrationBuilder.DropForeignKey(
                name: "f_k_tasks_services_service_id",
                table: "tasks");

            migrationBuilder.DropTable(
                name: "area_services");

            migrationBuilder.DropIndex(
                name: "i_x_tasks_service_id",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "i_x_slots_team_id",
                table: "slots");

            migrationBuilder.DropIndex(
                name: "i_x_services_service_type_is_active",
                table: "services");

            migrationBuilder.DropIndex(
                name: "i_x_pet_groups_area_id",
                table: "pet_groups");

            migrationBuilder.DropIndex(
                name: "i_x_employees_area_id",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "team_type",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "service_id",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "available_capacity",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "requires_area",
                table: "services");

            migrationBuilder.DropColumn(
                name: "service_type",
                table: "services");

            migrationBuilder.DropColumn(
                name: "area_id",
                table: "pet_groups");

            migrationBuilder.DropColumn(
                name: "area_id",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "pet_group_id",
                table: "customer_bookings",
                newName: "team_id");

            migrationBuilder.RenameIndex(
                name: "i_x_customer_bookings_pet_group_id",
                table: "customer_bookings",
                newName: "i_x_customer_bookings_team_id");

            migrationBuilder.AddColumn<string>(
                name: "applicable_days",
                table: "work_shifts",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "leader_id",
                table: "teams",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

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
                name: "status",
                table: "tasks",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "priority",
                table: "tasks",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldDefaultValue: "Medium");

            migrationBuilder.AddColumn<Guid>(
                name: "work_type_id",
                table: "services",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "work_type_id",
                table: "areas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "daily_tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_tasks", x => x.id);
                    table.ForeignKey(
                        name: "f_k_daily_tasks_tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_daily_tasks_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_pet_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "team_work_shifts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_shift_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_work_shifts", x => x.id);
                    table.ForeignKey(
                        name: "f_k_team_work_shifts_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_team_work_shifts_work_shifts_work_shift_id",
                        column: x => x.work_shift_id,
                        principalTable: "work_shifts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_types", x => x.id);
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
                name: "i_x_services_is_active",
                table: "services",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_services_work_type_id",
                table: "services",
                column: "work_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_areas_work_type_id",
                table: "areas",
                column: "work_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_daily_tasks_task_id",
                table: "daily_tasks",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "i_x_daily_tasks_team_id",
                table: "daily_tasks",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "i_x_service_pet_groups_pet_group_id",
                table: "service_pet_groups",
                column: "pet_group_id");

            migrationBuilder.CreateIndex(
                name: "i_x_service_pet_groups_service_id",
                table: "service_pet_groups",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "i_x_team_work_shifts_team_id",
                table: "team_work_shifts",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "i_x_team_work_shifts_work_shift_id",
                table: "team_work_shifts",
                column: "work_shift_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_areas_work_types_work_type_id",
                table: "areas",
                column: "work_type_id",
                principalTable: "work_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_customer_bookings_teams_team_id",
                table: "customer_bookings",
                column: "team_id",
                principalTable: "teams",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_areas_work_types_work_type_id",
                table: "areas");

            migrationBuilder.DropForeignKey(
                name: "f_k_customer_bookings_teams_team_id",
                table: "customer_bookings");

            migrationBuilder.DropForeignKey(
                name: "f_k_services_work_types_work_type_id",
                table: "services");

            migrationBuilder.DropForeignKey(
                name: "f_k_teams_areas_area_id",
                table: "teams");

            migrationBuilder.DropForeignKey(
                name: "f_k_teams_work_types_work_type_id",
                table: "teams");

            migrationBuilder.DropTable(
                name: "daily_tasks");

            migrationBuilder.DropTable(
                name: "service_pet_groups");

            migrationBuilder.DropTable(
                name: "team_work_shifts");

            migrationBuilder.DropTable(
                name: "work_types");

            migrationBuilder.DropIndex(
                name: "i_x_teams_area_id",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "i_x_teams_work_type_id",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "i_x_services_is_active",
                table: "services");

            migrationBuilder.DropIndex(
                name: "i_x_services_work_type_id",
                table: "services");

            migrationBuilder.DropIndex(
                name: "i_x_areas_work_type_id",
                table: "areas");

            migrationBuilder.DropColumn(
                name: "applicable_days",
                table: "work_shifts");

            migrationBuilder.DropColumn(
                name: "area_id",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "work_type_id",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "work_type_id",
                table: "services");

            migrationBuilder.DropColumn(
                name: "work_type_id",
                table: "areas");

            migrationBuilder.RenameColumn(
                name: "team_id",
                table: "customer_bookings",
                newName: "pet_group_id");

            migrationBuilder.RenameIndex(
                name: "i_x_customer_bookings_team_id",
                table: "customer_bookings",
                newName: "i_x_customer_bookings_pet_group_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "leader_id",
                table: "teams",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "team_type",
                table: "teams",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tasks",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "priority",
                table: "tasks",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Medium",
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<Guid>(
                name: "service_id",
                table: "tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "available_capacity",
                table: "slots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "team_id",
                table: "slots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "requires_area",
                table: "services",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "service_type",
                table: "services",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "area_id",
                table: "pet_groups",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "area_id",
                table: "employees",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "area_services",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    area_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    current_bookings = table.Column<int>(type: "integer", nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    max_capacity = table.Column<int>(type: "integer", nullable: false),
                    price_adjustment = table.Column<double>(type: "double precision", nullable: false),
                    special_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_area_services", x => x.id);
                    table.ForeignKey(
                        name: "f_k_area_services_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_area_services_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_service_id",
                table: "tasks",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_team_id",
                table: "slots",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "i_x_services_service_type_is_active",
                table: "services",
                columns: new[] { "service_type", "is_active" });

            migrationBuilder.CreateIndex(
                name: "i_x_pet_groups_area_id",
                table: "pet_groups",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "i_x_employees_area_id",
                table: "employees",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "i_x_area_services_area_id_service_id",
                table: "area_services",
                columns: new[] { "area_id", "service_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_area_services_service_id",
                table: "area_services",
                column: "service_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_customer_bookings_pet_groups_pet_group_id",
                table: "customer_bookings",
                column: "pet_group_id",
                principalTable: "pet_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_employees_areas_area_id",
                table: "employees",
                column: "area_id",
                principalTable: "areas",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_pet_groups_areas_area_id",
                table: "pet_groups",
                column: "area_id",
                principalTable: "areas",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_slots_teams_team_id",
                table: "slots",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_tasks_services_service_id",
                table: "tasks",
                column: "service_id",
                principalTable: "services",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
