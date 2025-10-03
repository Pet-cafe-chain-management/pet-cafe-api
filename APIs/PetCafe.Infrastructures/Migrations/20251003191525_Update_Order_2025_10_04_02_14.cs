using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_Order_2025_10_04_02_14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_customer_bookings_order_details_order_detail_id",
                table: "customer_bookings");

            migrationBuilder.DropTable(
                name: "customer_booking_slot");

            migrationBuilder.DropTable(
                name: "order_details");

            migrationBuilder.DropIndex(
                name: "i_x_customer_bookings_scheduled_date",
                table: "customer_bookings");

            migrationBuilder.DropIndex(
                name: "i_x_customer_bookings_service_id_scheduled_date",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "area_adjustment",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "base_amount",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "booking_number",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "completion_date",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "participants",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "payment_status",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "scheduled_date",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "special_requests",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "total_amount",
                table: "customer_bookings");

            migrationBuilder.RenameColumn(
                name: "scheduled_time",
                table: "customer_bookings",
                newName: "start_time");

            migrationBuilder.AddColumn<string>(
                name: "payment_data",
                table: "orders",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                table: "customer_bookings",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "end_time",
                table: "customer_bookings",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<Guid>(
                name: "pet_group_id",
                table: "customer_bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "slot_id",
                table: "customer_bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "product_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_amount = table.Column<double>(type: "double precision", nullable: false),
                    discount_amount = table.Column<double>(type: "double precision", nullable: false),
                    final_amount = table.Column<double>(type: "double precision", nullable: false),
                    notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_orders", x => x.id);
                    table.ForeignKey(
                        name: "f_k_product_orders_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "service_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_amount = table.Column<double>(type: "double precision", nullable: false),
                    discount_amount = table.Column<double>(type: "double precision", nullable: false),
                    final_amount = table.Column<double>(type: "double precision", nullable: false),
                    notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    order_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_orders", x => x.id);
                    table.ForeignKey(
                        name: "f_k_service_orders_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_order_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<double>(type: "double precision", nullable: false),
                    total_price = table.Column<double>(type: "double precision", nullable: false),
                    is_for_feeding = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    booking_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_order_details", x => x.id);
                    table.ForeignKey(
                        name: "f_k_product_order_details_product_orders_product_order_id",
                        column: x => x.product_order_id,
                        principalTable: "product_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_product_order_details_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_order_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: true),
                    slot_id = table.Column<Guid>(type: "uuid", nullable: true),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<double>(type: "double precision", nullable: false),
                    total_price = table.Column<double>(type: "double precision", nullable: false),
                    notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    booking_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_order_details", x => x.id);
                    table.ForeignKey(
                        name: "f_k_service_order_details_service_orders_service_order_id",
                        column: x => x.service_order_id,
                        principalTable: "service_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_service_order_details_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_service_order_details_slots_slot_id",
                        column: x => x.slot_id,
                        principalTable: "slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_pet_group_id",
                table: "customer_bookings",
                column: "pet_group_id");

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_service_id_slot_id",
                table: "customer_bookings",
                columns: new[] { "service_id", "slot_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_slot_id",
                table: "customer_bookings",
                column: "slot_id");

            migrationBuilder.CreateIndex(
                name: "i_x_product_order_details_product_id",
                table: "product_order_details",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "i_x_product_order_details_product_order_id_product_id",
                table: "product_order_details",
                columns: new[] { "product_order_id", "product_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_product_orders_order_id",
                table: "product_orders",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_service_order_details_service_id",
                table: "service_order_details",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "i_x_service_order_details_service_order_id",
                table: "service_order_details",
                column: "service_order_id");

            migrationBuilder.CreateIndex(
                name: "i_x_service_order_details_slot_id",
                table: "service_order_details",
                column: "slot_id");

            migrationBuilder.CreateIndex(
                name: "i_x_service_orders_order_date",
                table: "service_orders",
                column: "order_date");

            migrationBuilder.CreateIndex(
                name: "i_x_service_orders_order_id",
                table: "service_orders",
                column: "order_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "f_k_customer_bookings_pet_groups_pet_group_id",
                table: "customer_bookings",
                column: "pet_group_id",
                principalTable: "pet_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_customer_bookings_service_order_details_order_detail_id",
                table: "customer_bookings",
                column: "order_detail_id",
                principalTable: "service_order_details",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_customer_bookings_slots_slot_id",
                table: "customer_bookings",
                column: "slot_id",
                principalTable: "slots",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_customer_bookings_pet_groups_pet_group_id",
                table: "customer_bookings");

            migrationBuilder.DropForeignKey(
                name: "f_k_customer_bookings_service_order_details_order_detail_id",
                table: "customer_bookings");

            migrationBuilder.DropForeignKey(
                name: "f_k_customer_bookings_slots_slot_id",
                table: "customer_bookings");

            migrationBuilder.DropTable(
                name: "product_order_details");

            migrationBuilder.DropTable(
                name: "service_order_details");

            migrationBuilder.DropTable(
                name: "product_orders");

            migrationBuilder.DropTable(
                name: "service_orders");

            migrationBuilder.DropIndex(
                name: "i_x_customer_bookings_pet_group_id",
                table: "customer_bookings");

            migrationBuilder.DropIndex(
                name: "i_x_customer_bookings_service_id_slot_id",
                table: "customer_bookings");

            migrationBuilder.DropIndex(
                name: "i_x_customer_bookings_slot_id",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "payment_data",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "end_time",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "pet_group_id",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "slot_id",
                table: "customer_bookings");

            migrationBuilder.RenameColumn(
                name: "start_time",
                table: "customer_bookings",
                newName: "scheduled_time");

            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                table: "customer_bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "area_adjustment",
                table: "customer_bookings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "base_amount",
                table: "customer_bookings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "booking_number",
                table: "customer_bookings",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                table: "customer_bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "participants",
                table: "customer_bookings",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "payment_status",
                table: "customer_bookings",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "scheduled_date",
                table: "customer_bookings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "special_requests",
                table: "customer_bookings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "total_amount",
                table: "customer_bookings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

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
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_for_feeding = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    total_price = table.Column<double>(type: "double precision", nullable: false),
                    unit_price = table.Column<double>(type: "double precision", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_scheduled_date",
                table: "customer_bookings",
                column: "scheduled_date");

            migrationBuilder.CreateIndex(
                name: "i_x_customer_bookings_service_id_scheduled_date",
                table: "customer_bookings",
                columns: new[] { "service_id", "scheduled_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_customer_booking_slot_slots_id",
                table: "customer_booking_slot",
                column: "slots_id");

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

            migrationBuilder.AddForeignKey(
                name: "f_k_customer_bookings_order_details_order_detail_id",
                table: "customer_bookings",
                column: "order_detail_id",
                principalTable: "order_details",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
