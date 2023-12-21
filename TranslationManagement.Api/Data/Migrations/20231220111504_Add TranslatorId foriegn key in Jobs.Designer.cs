﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TranslationManagement.Api;

#nullable disable

namespace TranslationManagement.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231220111504_Add TranslatorId foriegn key in Jobs")]
    partial class AddTranslatorIdforiegnkeyinJobs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.24");

            modelBuilder.Entity("TranslationManagement.Api.Entities.TranslationJob", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerName")
                        .HasColumnType("TEXT");

                    b.Property<string>("OriginalContent")
                        .HasColumnType("TEXT");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TranslatedContent")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TranslatorId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TranslatorId");

                    b.ToTable("TranslationJobs");
                });

            modelBuilder.Entity("TranslationManagement.Api.Entities.Translator", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreditCardNumber")
                        .HasColumnType("TEXT");

                    b.Property<double>("HourlyRate")
                        .HasColumnType("REAL");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Translators");
                });

            modelBuilder.Entity("TranslationManagement.Api.Entities.TranslationJob", b =>
                {
                    b.HasOne("TranslationManagement.Api.Entities.Translator", "Translator")
                        .WithMany()
                        .HasForeignKey("TranslatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Translator");
                });
#pragma warning restore 612, 618
        }
    }
}