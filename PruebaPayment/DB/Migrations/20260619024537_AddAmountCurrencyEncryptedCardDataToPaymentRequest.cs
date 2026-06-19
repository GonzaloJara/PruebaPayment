using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaPayment.DB.Migrations
{
    /// <inheritdoc />
    public partial class AddAmountCurrencyEncryptedCardDataToPaymentRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "PaymentRequests",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "PaymentRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EncryptedCardData",
                table: "PaymentRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "PaymentRequests");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "PaymentRequests");

            migrationBuilder.DropColumn(
                name: "EncryptedCardData",
                table: "PaymentRequests");
        }
    }
}
