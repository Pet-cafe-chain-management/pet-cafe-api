using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Index_2025_12_10_23_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_work_shifts_name",
                table: "work_shifts");

            migrationBuilder.DropIndex(
                name: "i_x_vaccination_schedules_pet_id_scheduled_date",
                table: "vaccination_schedules");

            migrationBuilder.DropIndex(
                name: "i_x_vaccination_schedules_scheduled_date",
                table: "vaccination_schedules");

            migrationBuilder.DropIndex(
                name: "i_x_vaccination_records_pet_id_name_vaccination_date",
                table: "vaccination_records");

            migrationBuilder.DropIndex(
                name: "i_x_teams_name",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "i_x_tasks_status",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "i_x_services_is_active",
                table: "services");

            migrationBuilder.DropIndex(
                name: "i_x_services_name",
                table: "services");

            migrationBuilder.DropIndex(
                name: "i_x_service_orders_order_date",
                table: "service_orders");

            migrationBuilder.DropIndex(
                name: "i_x_products_category_id_is_active",
                table: "products");

            migrationBuilder.DropIndex(
                name: "i_x_products_name",
                table: "products");

            migrationBuilder.DropIndex(
                name: "i_x_product_order_details_product_order_id_product_id",
                table: "product_order_details");

            migrationBuilder.DropIndex(
                name: "i_x_product_categories_name",
                table: "product_categories");

            migrationBuilder.DropIndex(
                name: "i_x_pets_name",
                table: "pets");

            migrationBuilder.DropIndex(
                name: "i_x_pet_breeds_name_species_id",
                table: "pet_breeds");

            migrationBuilder.DropIndex(
                name: "i_x_orders_customer_id_order_date",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "i_x_orders_order_date",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "i_x_orders_order_number",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "i_x_notifications_account_id_is_read",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "i_x_notifications_notification_type_created_at",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "i_x_notifications_scheduled_send_date",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "i_x_leave_requests_employee_id_leave_date",
                table: "leave_requests");

            migrationBuilder.DropIndex(
                name: "i_x_leave_requests_status_reviewed_by",
                table: "leave_requests");

            migrationBuilder.DropIndex(
                name: "i_x_health_records_pet_id_check_date",
                table: "health_records");

            migrationBuilder.DropIndex(
                name: "i_x_employees_phone",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "i_x_employee_optional_work_shifts_employee_id_work_shift_id",
                table: "employee_optional_work_shifts");

            migrationBuilder.DropIndex(
                name: "i_x_customers_phone",
                table: "customers");

            migrationBuilder.DropIndex(
                name: "i_x_customer_bookings_customer_id_booking_date",
                table: "customer_bookings");

            migrationBuilder.DropIndex(
                name: "i_x_customer_bookings_service_id_slot_id",
                table: "customer_bookings");

            migrationBuilder.DropIndex(
                name: "i_x_areas_name",
                table: "areas");

            migrationBuilder.DropIndex(
                name: "i_x_accounts_email",
                table: "accounts");

            migrationBuilder.DropIndex(
                name: "i_x_accounts_role_is_active",
                table: "accounts");

            migrationBuilder.DropIndex(
                name: "i_x_accounts_username",
                table: "accounts");

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_schedules_pet_id",
                table: "vaccination_schedules",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_records_pet_id",
                table: "vaccination_records",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "i_x_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "i_x_product_order_details_product_order_id",
                table: "product_order_details",
                column: "product_order_id");

            migrationBuilder.CreateIndex(
                name: "i_x_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_account_id",
                table: "notifications",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_leave_requests_employee_id",
                table: "leave_requests",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "i_x_health_records_pet_id",
                table: "health_records",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "i_x_employee_optional_work_shifts_employee_id",
                table: "employee_optional_work_shifts",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_customer_id",
                table: "customer_bookings",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_service_id",
                table: "customer_bookings",
                column: "service_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_vaccination_schedules_pet_id",
                table: "vaccination_schedules");

            migrationBuilder.DropIndex(
                name: "i_x_vaccination_records_pet_id",
                table: "vaccination_records");

            migrationBuilder.DropIndex(
                name: "i_x_products_category_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "i_x_product_order_details_product_order_id",
                table: "product_order_details");

            migrationBuilder.DropIndex(
                name: "i_x_orders_customer_id",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "i_x_notifications_account_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "i_x_leave_requests_employee_id",
                table: "leave_requests");

            migrationBuilder.DropIndex(
                name: "i_x_health_records_pet_id",
                table: "health_records");

            migrationBuilder.DropIndex(
                name: "i_x_employee_optional_work_shifts_employee_id",
                table: "employee_optional_work_shifts");

            migrationBuilder.DropIndex(
                name: "i_x_customer_bookings_customer_id",
                table: "customer_bookings");

            migrationBuilder.DropIndex(
                name: "i_x_customer_bookings_service_id",
                table: "customer_bookings");

            migrationBuilder.CreateIndex(
                name: "i_x_work_shifts_name",
                table: "work_shifts",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_schedules_pet_id_scheduled_date",
                table: "vaccination_schedules",
                columns: new[] { "pet_id", "scheduled_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_schedules_scheduled_date",
                table: "vaccination_schedules",
                column: "scheduled_date");

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_records_pet_id_name_vaccination_date",
                table: "vaccination_records",
                columns: new[] { "pet_id", "name", "vaccination_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_teams_name",
                table: "teams",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_status",
                table: "tasks",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "i_x_services_is_active",
                table: "services",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_services_name",
                table: "services",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "i_x_service_orders_order_date",
                table: "service_orders",
                column: "order_date");

            migrationBuilder.CreateIndex(
                name: "i_x_products_category_id_is_active",
                table: "products",
                columns: new[] { "category_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "i_x_products_name",
                table: "products",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "i_x_product_order_details_product_order_id_product_id",
                table: "product_order_details",
                columns: new[] { "product_order_id", "product_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_product_categories_name",
                table: "product_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_pets_name",
                table: "pets",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "i_x_pet_breeds_name_species_id",
                table: "pet_breeds",
                columns: new[] { "name", "species_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_orders_customer_id_order_date",
                table: "orders",
                columns: new[] { "customer_id", "order_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_orders_order_date",
                table: "orders",
                column: "order_date");

            migrationBuilder.CreateIndex(
                name: "i_x_orders_order_number",
                table: "orders",
                column: "order_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_account_id_is_read",
                table: "notifications",
                columns: new[] { "account_id", "is_read" });

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_notification_type_created_at",
                table: "notifications",
                columns: new[] { "notification_type", "created_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_scheduled_send_date",
                table: "notifications",
                column: "scheduled_send_date");

            migrationBuilder.CreateIndex(
                name: "i_x_leave_requests_employee_id_leave_date",
                table: "leave_requests",
                columns: new[] { "employee_id", "leave_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_leave_requests_status_reviewed_by",
                table: "leave_requests",
                columns: new[] { "status", "reviewed_by" });

            migrationBuilder.CreateIndex(
                name: "i_x_health_records_pet_id_check_date",
                table: "health_records",
                columns: new[] { "pet_id", "check_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_employees_phone",
                table: "employees",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_employee_optional_work_shifts_employee_id_work_shift_id",
                table: "employee_optional_work_shifts",
                columns: new[] { "employee_id", "work_shift_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_customers_phone",
                table: "customers",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_customer_id_booking_date",
                table: "customer_bookings",
                columns: new[] { "customer_id", "booking_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_service_id_slot_id",
                table: "customer_bookings",
                columns: new[] { "service_id", "slot_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_areas_name",
                table: "areas",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "i_x_accounts_email",
                table: "accounts",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_accounts_role_is_active",
                table: "accounts",
                columns: new[] { "role", "is_active" });

            migrationBuilder.CreateIndex(
                name: "i_x_accounts_username",
                table: "accounts",
                column: "username",
                unique: true);
        }
    }
}
