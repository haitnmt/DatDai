using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace haihv.DatDai.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DvhcDtos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    MaKyHieu = table.Column<string>(type: "text", nullable: false),
                    TenGiaTri = table.Column<string>(type: "text", nullable: false),
                    MaXa = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    MaHuyen = table.Column<string>(type: "text", nullable: true),
                    MaTinh = table.Column<string>(type: "text", nullable: true),
                    Cap = table.Column<int>(type: "integer", nullable: false),
                    LoaiHinh = table.Column<string>(type: "text", nullable: false),
                    NgayHieuLuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HieuLuc = table.Column<bool>(type: "boolean", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DvhcDtos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DvhcDtos");
        }
    }
}
