using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IdentityMicroService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6cb8651b-2e27-4b3a-b47e-8b33eddc9705");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d3124d4b-0b36-4972-bfb0-e6b196c170a8");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhotoId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4622e72d-f609-4964-b19d-4ce611cbc71a", null, "Doctor", "DOCTOR" },
                    { "46d81a0b-9489-41d1-927c-249ffab0caba", null, "Patient", "PATIENT" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "77619ae5-1ac9-4ee1-84aa-cc32dab2bb68",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "CreatedBy", "PasswordHash", "PhotoId", "UpdatedBy" },
                values: new object[] { "39d0d5b0-58c0-4a19-8e48-979f0d53f22c", new DateTime(2023, 1, 10, 8, 35, 20, 914, DateTimeKind.Utc).AddTicks(8339), null, "AQAAAAIAAYagAAAAEDPjTxLDF0B9wyIyyDV6lzmEyCIhVXUQzosO5vxeS0VUJyt3366iFncCwE9trPFgQg==", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4622e72d-f609-4964-b19d-4ce611cbc71a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "46d81a0b-9489-41d1-927c-249ffab0caba");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedBy",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PhotoId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6cb8651b-2e27-4b3a-b47e-8b33eddc9705", null, "Patient", "PATIENT" },
                    { "d3124d4b-0b36-4972-bfb0-e6b196c170a8", null, "Doctor", "DOCTOR" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "77619ae5-1ac9-4ee1-84aa-cc32dab2bb68",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "CreatedBy", "PasswordHash", "PhotoId", "UpdatedBy" },
                values: new object[] { "8d900c1d-91c9-4f37-90d9-a2de11c63ccd", new DateTime(2022, 11, 25, 19, 6, 14, 807, DateTimeKind.Utc).AddTicks(4763), null, "AQAAAAIAAYagAAAAEJq15/54PDvaLSxG76nvcPHmriYg+AYAdHmybU45dLwtDAUymIpXb2Ebwurpq+WcRg==", 0, null });
        }
    }
}
