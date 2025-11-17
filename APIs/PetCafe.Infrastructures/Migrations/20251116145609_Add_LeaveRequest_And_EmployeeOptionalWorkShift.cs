using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Add_LeaveRequest_And_EmployeeOptionalWorkShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "employee_optional_work_shifts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_shift_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_optional_work_shifts", x => x.id);
                    table.ForeignKey(
                        name: "f_k_employee_optional_work_shifts_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_employee_optional_work_shifts_work_shifts_work_shift_id",
                        column: x => x.work_shift_id,
                        principalTable: "work_shifts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "leave_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    replacement_employee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    leave_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    leave_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reviewed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    review_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_requests", x => x.id);
                    table.ForeignKey(
                        name: "f_k_leave_requests_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_leave_requests_employees_replacement_employee_id",
                        column: x => x.replacement_employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_leave_requests_employees_reviewed_by",
                        column: x => x.reviewed_by,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_employee_optional_work_shifts_employee_id_work_shift_id",
                table: "employee_optional_work_shifts",
                columns: new[] { "employee_id", "work_shift_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_employee_optional_work_shifts_work_shift_id",
                table: "employee_optional_work_shifts",
                column: "work_shift_id");

            migrationBuilder.CreateIndex(
                name: "i_x_leave_requests_employee_id_leave_date",
                table: "leave_requests",
                columns: new[] { "employee_id", "leave_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_leave_requests_replacement_employee_id",
                table: "leave_requests",
                column: "replacement_employee_id");

            migrationBuilder.CreateIndex(
                name: "i_x_leave_requests_reviewed_by",
                table: "leave_requests",
                column: "reviewed_by");

            migrationBuilder.CreateIndex(
                name: "i_x_leave_requests_status_reviewed_by",
                table: "leave_requests",
                columns: new[] { "status", "reviewed_by" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_optional_work_shifts");

            migrationBuilder.DropTable(
                name: "leave_requests");
        }
    }
}
