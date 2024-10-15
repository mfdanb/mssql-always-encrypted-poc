﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MssqlAlwaysEncryptedPoc.ExampleDb;

#nullable disable

namespace MssqlAlwaysEncryptedPoc.ExampleDb.Migrations.Migrations
{
    [DbContext(typeof(EncryptedExampleDbContext))]
    [Migration("20241015151915_InitialEncryptedExampleDb")]
    partial class InitialEncryptedExampleDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-rc.2.24474.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MssqlAlwaysEncryptedPoc.ExampleDb.EncryptedExample", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("EncryptedExamples");
                });
#pragma warning restore 612, 618
        }
    }
}
