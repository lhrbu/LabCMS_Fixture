﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LabCMS.EquipmentDomain.Server.Migrations.UsageRecordsRecycleBinMigrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoftDeletedUsageRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    User = table.Column<string>(type: "TEXT", nullable: true),
                    TestNo = table.Column<string>(type: "TEXT", nullable: true),
                    EquipmentNo = table.Column<string>(type: "TEXT", nullable: true),
                    TestType = table.Column<string>(type: "TEXT", nullable: true),
                    ProjectName = table.Column<string>(type: "TEXT", nullable: true),
                    StartTime = table.Column<long>(type: "INTEGER", nullable: true),
                    EndTime = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftDeletedUsageRecords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoftDeletedUsageRecords");
        }
    }
}
