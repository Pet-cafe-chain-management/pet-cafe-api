using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Add_ServiceFeedback_2025_11_01_03_12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "feedback_comment",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "feedback_date",
                table: "customer_bookings");

            migrationBuilder.DropColumn(
                name: "feedback_rating",
                table: "customer_bookings");

            migrationBuilder.CreateTable(
                name: "service_feedbacks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    feedback_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_feedbacks", x => x.id);
                    table.ForeignKey(
                        name: "f_k_service_feedbacks_customer_bookings_customer_booking_id",
                        column: x => x.customer_booking_id,
                        principalTable: "customer_bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_service_feedbacks_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_service_feedbacks_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_service_feedbacks_customer_booking_id",
                table: "service_feedbacks",
                column: "customer_booking_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_service_feedbacks_customer_id",
                table: "service_feedbacks",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "i_x_service_feedbacks_service_id",
                table: "service_feedbacks",
                column: "service_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "service_feedbacks");

            migrationBuilder.AddColumn<string>(
                name: "feedback_comment",
                table: "customer_bookings",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "feedback_date",
                table: "customer_bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "feedback_rating",
                table: "customer_bookings",
                type: "integer",
                nullable: true);
        }
    }
}
