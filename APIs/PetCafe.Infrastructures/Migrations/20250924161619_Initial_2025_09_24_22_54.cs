using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Initial_2025_09_24_22_54 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "areas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    max_capacity = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_areas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pet_species",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_pet_species", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    base_price = table.Column<double>(type: "double precision", nullable: false),
                    max_participants = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    service_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    requires_area = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "work_shifts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_shifts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    loyalty_points = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.id);
                    table.ForeignKey(
                        name: "f_k_customers_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    notification_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    priority = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Normal"),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    read_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reference_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reference_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    scheduled_send_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sent_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "f_k_notifications_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    hire_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    salary = table.Column<double>(type: "double precision", nullable: true),
                    position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    area_id = table.Column<Guid>(type: "uuid", nullable: true),
                    leader_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.id);
                    table.ForeignKey(
                        name: "f_k_employees_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_employees_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "areas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_employees_employees_leader_id",
                        column: x => x.leader_id,
                        principalTable: "employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "pet_breeds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    species_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    average_weight = table.Column<double>(type: "double precision", nullable: false),
                    average_lifespan = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pet_breeds", x => x.id);
                    table.ForeignKey(
                        name: "f_k_pet_breeds_pet_species_species_id",
                        column: x => x.species_id,
                        principalTable: "pet_species",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vaccine_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    species_id = table.Column<Guid>(type: "uuid", nullable: true),
                    interval_months = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    product_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    cost = table.Column<double>(type: "double precision", nullable: true),
                    stock_quantity = table.Column<int>(type: "integer", nullable: false),
                    min_stock_level = table.Column<int>(type: "integer", nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_for_pets = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                    table.ForeignKey(
                        name: "f_k_products_product_categories_product_category_id",
                        column: x => x.product_category_id,
                        principalTable: "product_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "area_services",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    area_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    max_capacity = table.Column<int>(type: "integer", nullable: false),
                    current_bookings = table.Column<int>(type: "integer", nullable: false),
                    price_adjustment = table.Column<double>(type: "double precision", nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    special_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "employee_schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_shift_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Scheduled"),
                    check_in_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    check_out_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_schedules", x => x.id);
                    table.ForeignKey(
                        name: "f_k_employee_schedules_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_employee_schedules_work_shifts_work_shift_id",
                        column: x => x.work_shift_id,
                        principalTable: "work_shifts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    order_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total_amount = table.Column<double>(type: "double precision", nullable: false),
                    discount_amount = table.Column<double>(type: "double precision", nullable: false),
                    final_amount = table.Column<double>(type: "double precision", nullable: false),
                    payment_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    payment_method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                    table.ForeignKey(
                        name: "f_k_orders_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_orders_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    team_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    leader_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.id);
                    table.ForeignKey(
                        name: "f_k_teams_employees_leader_id",
                        column: x => x.leader_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "pet_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    max_capacity = table.Column<int>(type: "integer", nullable: false),
                    pet_species_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pet_breed_id = table.Column<Guid>(type: "uuid", nullable: true),
                    area_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pet_groups", x => x.id);
                    table.ForeignKey(
                        name: "f_k_pet_groups_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "areas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_pet_groups_pet_breeds_pet_breed_id",
                        column: x => x.pet_breed_id,
                        principalTable: "pet_breeds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_pet_groups_pet_species_pet_species_id",
                        column: x => x.pet_species_id,
                        principalTable: "pet_species",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_code = table.Column<double>(type: "double precision", nullable: false),
                    amount = table.Column<double>(type: "double precision", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    account_number = table.Column<string>(type: "text", nullable: true),
                    reference = table.Column<string>(type: "text", nullable: true),
                    transaction_date_time = table.Column<string>(type: "text", nullable: true),
                    currency = table.Column<string>(type: "text", nullable: true),
                    payment_link_id = table.Column<string>(type: "text", nullable: true),
                    code = table.Column<string>(type: "text", nullable: true),
                    desc = table.Column<string>(type: "text", nullable: true),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.id);
                    table.ForeignKey(
                        name: "f_k_transactions_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "team_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_members", x => x.id);
                    table.ForeignKey(
                        name: "f_k_team_members_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_team_members_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    species_id = table.Column<Guid>(type: "uuid", nullable: false),
                    breed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: true),
                    age = table.Column<int>(type: "integer", nullable: false),
                    gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    color = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    weight = table.Column<double>(type: "double precision", nullable: true),
                    preferences = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    special_notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    image_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    arrival_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pets", x => x.id);
                    table.ForeignKey(
                        name: "f_k_pets_pet_breeds_breed_id",
                        column: x => x.breed_id,
                        principalTable: "pet_breeds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_pets_pet_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "pet_groups",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_pets_pet_species_species_id",
                        column: x => x.species_id,
                        principalTable: "pet_species",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "slots",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    area_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_gourp_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    max_capacity = table.Column<int>(type: "integer", nullable: false),
                    available_capacity = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    special_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    slot_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slots", x => x.id);
                    table.ForeignKey(
                        name: "f_k_slots_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_slots_pet_groups_pet_gourp_id",
                        column: x => x.pet_gourp_id,
                        principalTable: "pet_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_slots_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_slots_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "health_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    check_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    weight = table.Column<double>(type: "double precision", nullable: true),
                    temperature = table.Column<double>(type: "double precision", nullable: true),
                    health_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    symptoms = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    treatment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    veterinarian = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    next_check_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_health_records", x => x.id);
                    table.ForeignKey(
                        name: "f_k_health_records_pets_pet_id",
                        column: x => x.pet_id,
                        principalTable: "pets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    priority = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Medium"),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    estimated_hours = table.Column<int>(type: "integer", nullable: true),
                    actual_hours = table.Column<int>(type: "integer", nullable: true),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    area_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pet_group_id = table.Column<Guid>(type: "uuid", nullable: true),
                    service_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pet_id = table.Column<Guid>(type: "uuid", nullable: true),
                    work_shift_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.id);
                    table.ForeignKey(
                        name: "f_k_tasks_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_tasks_pet_groups_pet_group_id",
                        column: x => x.pet_group_id,
                        principalTable: "pet_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_tasks_pets_pet_id",
                        column: x => x.pet_id,
                        principalTable: "pets",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_tasks_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_tasks_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_tasks_work_shifts_work_shift_id",
                        column: x => x.work_shift_id,
                        principalTable: "work_shifts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vaccination_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vaccine_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vaccination_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    next_due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    veterinarian = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    clinic_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    batch_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccination_records", x => x.id);
                    table.ForeignKey(
                        name: "f_k_vaccination_records_pets_pet_id",
                        column: x => x.pet_id,
                        principalTable: "pets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_vaccination_records_vaccine_types_vaccine_type_id",
                        column: x => x.vaccine_type_id,
                        principalTable: "vaccine_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vaccination_schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vaccine_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scheduled_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    completed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccination_schedules", x => x.id);
                    table.ForeignKey(
                        name: "f_k_vaccination_schedules_pets_pet_id",
                        column: x => x.pet_id,
                        principalTable: "pets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_vaccination_schedules_vaccine_types_vaccine_type_id",
                        column: x => x.vaccine_type_id,
                        principalTable: "vaccine_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: true),
                    service_id = table.Column<Guid>(type: "uuid", nullable: true),
                    slot_id = table.Column<Guid>(type: "uuid", nullable: true),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<double>(type: "double precision", nullable: false),
                    total_price = table.Column<double>(type: "double precision", nullable: false),
                    is_for_feeding = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_details", x => x.id);
                    table.ForeignKey(
                        name: "f_k_order_details_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_order_details_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_order_details_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_order_details_slots_slot_id",
                        column: x => x.slot_id,
                        principalTable: "slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "task_assignments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completion_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "customer_bookings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_detail_id = table.Column<Guid>(type: "uuid", nullable: false),
                    booking_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    booking_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    scheduled_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    scheduled_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    participants = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    total_amount = table.Column<double>(type: "double precision", nullable: false),
                    base_amount = table.Column<double>(type: "double precision", nullable: false),
                    area_adjustment = table.Column<double>(type: "double precision", nullable: false),
                    payment_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    booking_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    special_requests = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    completion_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    feedback_rating = table.Column<int>(type: "integer", nullable: true),
                    feedback_comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    feedback_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_bookings", x => x.id);
                    table.ForeignKey(
                        name: "f_k_customer_bookings_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_customer_bookings_order_details_order_detail_id",
                        column: x => x.order_detail_id,
                        principalTable: "order_details",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_customer_bookings_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customer_booking_slot",
                columns: table => new
                {
                    customer_bookings_id = table.Column<Guid>(type: "uuid", nullable: false),
                    slots_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_booking_slot", x => new { x.customer_bookings_id, x.slots_id });
                    table.ForeignKey(
                        name: "f_k_customer_booking_slot_customer_bookings_customer_bookings_id",
                        column: x => x.customer_bookings_id,
                        principalTable: "customer_bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_customer_booking_slot_slots_slots_id",
                        column: x => x.slots_id,
                        principalTable: "slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "i_x_area_services_area_id_service_id",
                table: "area_services",
                columns: new[] { "area_id", "service_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_area_services_service_id",
                table: "area_services",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "i_x_areas_name",
                table: "areas",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "i_x_customer_booking_slot_slots_id",
                table: "customer_booking_slot",
                column: "slots_id");

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_customer_id_booking_date",
                table: "customer_bookings",
                columns: new[] { "customer_id", "booking_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_order_detail_id",
                table: "customer_bookings",
                column: "order_detail_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_scheduled_date",
                table: "customer_bookings",
                column: "scheduled_date");

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_service_id_scheduled_date",
                table: "customer_bookings",
                columns: new[] { "service_id", "scheduled_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_customers_account_id",
                table: "customers",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_customers_phone",
                table: "customers",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_employee_schedules_employee_id_work_date",
                table: "employee_schedules",
                columns: new[] { "employee_id", "work_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_employee_schedules_work_date",
                table: "employee_schedules",
                column: "work_date");

            migrationBuilder.CreateIndex(
                name: "i_x_employee_schedules_work_shift_id",
                table: "employee_schedules",
                column: "work_shift_id");

            migrationBuilder.CreateIndex(
                name: "i_x_employees_account_id",
                table: "employees",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_employees_area_id",
                table: "employees",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "i_x_employees_leader_id",
                table: "employees",
                column: "leader_id");

            migrationBuilder.CreateIndex(
                name: "i_x_employees_phone",
                table: "employees",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_health_records_pet_id_check_date",
                table: "health_records",
                columns: new[] { "pet_id", "check_date" });

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
                name: "i_x_order_details_order_id_product_id",
                table: "order_details",
                columns: new[] { "order_id", "product_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_order_details_product_id",
                table: "order_details",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_details_service_id",
                table: "order_details",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_details_slot_id",
                table: "order_details",
                column: "slot_id");

            migrationBuilder.CreateIndex(
                name: "i_x_orders_customer_id_order_date",
                table: "orders",
                columns: new[] { "customer_id", "order_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_orders_employee_id",
                table: "orders",
                column: "employee_id");

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
                name: "i_x_pet_breeds_name_species_id",
                table: "pet_breeds",
                columns: new[] { "name", "species_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_pet_breeds_species_id",
                table: "pet_breeds",
                column: "species_id");

            migrationBuilder.CreateIndex(
                name: "i_x_pet_groups_area_id",
                table: "pet_groups",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "i_x_pet_groups_pet_breed_id",
                table: "pet_groups",
                column: "pet_breed_id");

            migrationBuilder.CreateIndex(
                name: "i_x_pet_groups_pet_species_id",
                table: "pet_groups",
                column: "pet_species_id");

            migrationBuilder.CreateIndex(
                name: "i_x_pets_breed_id",
                table: "pets",
                column: "breed_id");

            migrationBuilder.CreateIndex(
                name: "i_x_pets_group_id",
                table: "pets",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "i_x_pets_name",
                table: "pets",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "i_x_pets_species_id",
                table: "pets",
                column: "species_id");

            migrationBuilder.CreateIndex(
                name: "i_x_product_categories_name",
                table: "product_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_products_name",
                table: "products",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "i_x_products_product_category_id_is_active",
                table: "products",
                columns: new[] { "product_category_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "i_x_services_name",
                table: "services",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "i_x_services_service_type_is_active",
                table: "services",
                columns: new[] { "service_type", "is_active" });

            migrationBuilder.CreateIndex(
                name: "i_x_slots_area_id",
                table: "slots",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_pet_gourp_id",
                table: "slots",
                column: "pet_gourp_id");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_service_id",
                table: "slots",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "i_x_slots_team_id",
                table: "slots",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "i_x_task_assignments_employee_id_status",
                table: "task_assignments",
                columns: new[] { "employee_id", "status" });

            migrationBuilder.CreateIndex(
                name: "i_x_task_assignments_task_id_employee_id",
                table: "task_assignments",
                columns: new[] { "task_id", "employee_id" },
                unique: true);

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
                name: "i_x_tasks_service_id",
                table: "tasks",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_team_id_status",
                table: "tasks",
                columns: new[] { "team_id", "status" });

            migrationBuilder.CreateIndex(
                name: "i_x_tasks_work_shift_id",
                table: "tasks",
                column: "work_shift_id");

            migrationBuilder.CreateIndex(
                name: "i_x_team_members_employee_id",
                table: "team_members",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "i_x_team_members_team_id_employee_id",
                table: "team_members",
                columns: new[] { "team_id", "employee_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_teams_leader_id",
                table: "teams",
                column: "leader_id");

            migrationBuilder.CreateIndex(
                name: "i_x_teams_name",
                table: "teams",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_transactions_order_id",
                table: "transactions",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_records_pet_id_vaccine_type_id_vaccination_date",
                table: "vaccination_records",
                columns: new[] { "pet_id", "vaccine_type_id", "vaccination_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_records_vaccine_type_id",
                table: "vaccination_records",
                column: "vaccine_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_schedules_pet_id_scheduled_date",
                table: "vaccination_schedules",
                columns: new[] { "pet_id", "scheduled_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_schedules_scheduled_date",
                table: "vaccination_schedules",
                column: "scheduled_date");

            migrationBuilder.CreateIndex(
                name: "i_x_vaccination_schedules_vaccine_type_id",
                table: "vaccination_schedules",
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

            migrationBuilder.CreateIndex(
                name: "i_x_work_shifts_name",
                table: "work_shifts",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "area_services");

            migrationBuilder.DropTable(
                name: "customer_booking_slot");

            migrationBuilder.DropTable(
                name: "employee_schedules");

            migrationBuilder.DropTable(
                name: "health_records");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "task_assignments");

            migrationBuilder.DropTable(
                name: "team_members");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "vaccination_records");

            migrationBuilder.DropTable(
                name: "vaccination_schedules");

            migrationBuilder.DropTable(
                name: "customer_bookings");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "vaccine_types");

            migrationBuilder.DropTable(
                name: "order_details");

            migrationBuilder.DropTable(
                name: "pets");

            migrationBuilder.DropTable(
                name: "work_shifts");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "slots");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "product_categories");

            migrationBuilder.DropTable(
                name: "pet_groups");

            migrationBuilder.DropTable(
                name: "services");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "pet_breeds");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "pet_species");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "areas");
        }
    }
}
