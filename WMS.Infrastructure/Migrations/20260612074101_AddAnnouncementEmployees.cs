using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnouncementEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop manual columns not tracked by EF Core previously
            migrationBuilder.Sql(@"
                DECLARE @ConstraintName nvarchar(200)
                SELECT @ConstraintName = Name FROM sys.default_constraints
                WHERE parent_object_id = object_id('Announcements') AND parent_column_id = columnproperty(object_id('Announcements'), 'TargetAudience', 'ColumnId')
                IF @ConstraintName IS NOT NULL
                    EXEC('ALTER TABLE Announcements DROP CONSTRAINT ' + @ConstraintName)
            ");
            migrationBuilder.Sql("IF COL_LENGTH('Announcements', 'TargetAudience') IS NOT NULL ALTER TABLE Announcements DROP COLUMN TargetAudience;");
            migrationBuilder.Sql("IF COL_LENGTH('Announcements', 'TargetDepartmentId') IS NOT NULL ALTER TABLE Announcements DROP COLUMN TargetDepartmentId;");
            migrationBuilder.Sql("IF COL_LENGTH('Announcements', 'TargetEmployeeIds') IS NOT NULL ALTER TABLE Announcements DROP COLUMN TargetEmployeeIds;");

            migrationBuilder.CreateTable(
                name: "AnnouncementEmployees",
                columns: table => new
                {
                    AnnouncementId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementEmployees", x => new { x.AnnouncementId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_AnnouncementEmployees_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "AnnouncementId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnouncementEmployees_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2026, 6, 12, 13, 11, 1, 12, DateTimeKind.Local).AddTicks(9762));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2026, 6, 12, 13, 11, 1, 12, DateTimeKind.Local).AddTicks(9783));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2026, 6, 12, 13, 11, 1, 12, DateTimeKind.Local).AddTicks(9784));

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PhoneNumber",
                table: "Employees",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementEmployees_EmployeeId",
                table: "AnnouncementEmployees",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnouncementEmployees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PhoneNumber",
                table: "Employees");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2026, 6, 10, 20, 11, 12, 751, DateTimeKind.Local).AddTicks(6417));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2026, 6, 10, 20, 11, 12, 751, DateTimeKind.Local).AddTicks(6434));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2026, 6, 10, 20, 11, 12, 751, DateTimeKind.Local).AddTicks(6436));
        }
    }
}
