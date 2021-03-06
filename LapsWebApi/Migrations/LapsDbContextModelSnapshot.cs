﻿// <auto-generated />
using LapsWebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace LapsWebApi.Migrations
{
    [DbContext(typeof(LapsDbContext))]
    partial class LapsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("LapsWebApi.Models.Track", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(255);

                    b.Property<string>("Coordinates")
                        .IsRequired();

                    b.Property<double>("Distance");

                    b.Property<string>("Thumbnail")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("Id");

                    b.ToTable("Tracks");
                });
#pragma warning restore 612, 618
        }
    }
}
