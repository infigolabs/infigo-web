﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UMServer.Common;

#nullable disable

namespace UMServer.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20240323111935_init-database")]
    partial class initdatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.17")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UMServer.Models.Account", b =>
                {
                    b.Property<string>("AccountId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeviceActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsExpired")
                        .HasColumnType("bit");

                    b.Property<string>("LicenseKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OS")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OSVersion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlanId")
                        .HasColumnType("int");

                    b.Property<string>("ProductVersion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("SubscriptionEnd")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("SubscriptionStart")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("SubscriptionStatus")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AccountId");

                    b.HasIndex("PlanId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("UMServer.Models.Plan", b =>
                {
                    b.Property<int>("PlanId")
                        .HasColumnType("int");

                    b.Property<string>("PlanDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlanLength")
                        .HasColumnType("int");

                    b.Property<double>("PlanPrice")
                        .HasColumnType("float");

                    b.HasKey("PlanId");

                    b.ToTable("Plans");
                });

            modelBuilder.Entity("UMServer.Models.PremiumUser", b =>
                {
                    b.Property<string>("AccountId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlanId")
                        .HasColumnType("int");

                    b.ToTable("PremiumUsers");
                });

            modelBuilder.Entity("UMServer.Models.Account", b =>
                {
                    b.HasOne("UMServer.Models.Plan", "Plan")
                        .WithMany()
                        .HasForeignKey("PlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Plan");
                });
#pragma warning restore 612, 618
        }
    }
}
