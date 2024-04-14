﻿// <auto-generated />
using System;
using EchiBackendServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EchiBackendServices.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240414150833_AddedItBack")]
    partial class AddedItBack
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EchiBackendServices.Models.Agency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AgencyName")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Agencies");
                });

            modelBuilder.Entity("EchiBackendServices.Models.Agent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AgencyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("AgentEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("AgentName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("AgentPhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Agents");
                });

            modelBuilder.Entity("EchiBackendServices.Models.ClientModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AgencyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("AgentEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("AgentName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("AgentPhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ClientAddressCity")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ClientAddressLineOne")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ClientAddressLineTwo")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ClientAddressState")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ClientAddressZipCode")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ClientEmailAddress")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ClientFirstName")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ClientLastName")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ClientPhoneNumber")
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("DoNotIncludeRadonAddendum")
                        .HasColumnType("bit");

                    b.Property<string>("Fee")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Guid")
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IncludeRadonAddendum")
                        .HasColumnType("bit");

                    b.Property<string>("InspectionAddressCity")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("InspectionAddressLineOne")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("InspectionAddressLineTwo")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("InspectionAddressState")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("InspectionAddressZipCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("InspectionDateString")
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool?>("IsArchived")
                        .HasColumnType("bit");

                    b.Property<string>("MainInspectionImageFileName")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("MainInspectionImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PresentAtInspection")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("RadonFee")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("SerializedPageMaster")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });
#pragma warning restore 612, 618
        }
    }
}
