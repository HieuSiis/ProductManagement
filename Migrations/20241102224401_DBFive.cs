using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductManagement.Migrations
{
    /// <inheritdoc />
    public partial class DBFive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.AlterColumn<DateTime>(
			name: "CreatedAt",
			table: "Products",
			nullable: false,
			defaultValueSql: "SYSDATETIME()",
			oldClrType: typeof(DateTime),
			oldNullable: false);

			migrationBuilder.AlterColumn<DateTime>(
			name: "UpdatedAt",
			table: "Products",
			nullable: false,
			defaultValueSql: "SYSDATETIME()",
			oldClrType: typeof(DateTime),
			oldNullable: false);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
