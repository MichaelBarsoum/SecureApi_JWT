using Microsoft.EntityFrameworkCore.Migrations;
using Secure_Api_Using_JWT.Helpers;

#nullable disable

namespace Secure_Api_Using_JWT.Migrations
{
    public partial class SeedUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id" , "Name" , "NormalizedName" , "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString() , UserRoles.USER.ToString() , "User".ToUpper() , Guid.NewGuid().ToString() }
                );
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), UserRoles.ADMIN.ToString(), "admin".ToUpper(), Guid.NewGuid().ToString() }
                );
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE * FROM [AspNetRoles]");
        }
    }
}
