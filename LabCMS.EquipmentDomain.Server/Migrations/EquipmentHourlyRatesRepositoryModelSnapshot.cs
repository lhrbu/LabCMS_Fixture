﻿// <auto-generated />
using LabCMS.EquipmentDomain.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LabCMS.EquipmentDomain.Server.Migrations
{
    [DbContext(typeof(EquipmentHourlyRatesRepository))]
    partial class EquipmentHourlyRatesRepositoryModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("LabCMS.EquipmentDomain.Shared.Models.EquipmentHourlyRate", b =>
                {
                    b.Property<string>("EquipmentNo")
                        .HasColumnType("TEXT");

                    b.Property<string>("EquipmentName")
                        .HasColumnType("TEXT");

                    b.Property<string>("HourlyRate")
                        .HasColumnType("TEXT");

                    b.Property<string>("MachineCategory")
                        .HasColumnType("TEXT");

                    b.HasKey("EquipmentNo");

                    b.ToTable("EquipmentHourlyRates");
                });
#pragma warning restore 612, 618
        }
    }
}
