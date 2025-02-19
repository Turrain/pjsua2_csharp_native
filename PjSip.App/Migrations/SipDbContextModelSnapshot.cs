﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PjSip.App.Data;

#nullable disable

namespace PjSip.App.Migrations
{
    [DbContext(typeof(SipDbContext))]
    partial class SipDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            modelBuilder.Entity("PjSip.App.Models.AgentConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AgentId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("DATETIME('now')");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Priority")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AgentConfigs");
                });

            modelBuilder.Entity("PjSip.App.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Sender")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("PjSip.App.Models.SipAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("AgentId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Domain")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RegistrarUri")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.HasIndex("AgentId");

                    b.ToTable("SipAccounts");
                });

            modelBuilder.Entity("PjSip.App.Models.SipCall", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CallId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("EndedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("RemoteUri")
                        .IsRequired()
                        .HasMaxLength(511)
                        .HasColumnType("TEXT");

                    b.Property<int>("SipAccountId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SipAccountId");

                    b.ToTable("SipCalls");
                });

            modelBuilder.Entity("PjSip.App.Models.AgentConfig", b =>
                {
                    b.OwnsOne("PjSip.App.Models.AgentConfig+AuralisConfig", "Auralis", b1 =>
                        {
                            b1.Property<int>("AgentConfigId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("ApiKey")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<bool>("EnableAnalytics")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Endpoint")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<int>("Timeout")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER")
                                .HasDefaultValue(30);

                            b1.HasKey("AgentConfigId");

                            b1.ToTable("AgentConfigs");

                            b1.WithOwner()
                                .HasForeignKey("AgentConfigId");
                        });

                    b.OwnsOne("PjSip.App.Models.AgentConfig+LLMConfig", "LLM", b1 =>
                        {
                            b1.Property<int>("AgentConfigId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("MaxTokens")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER")
                                .HasDefaultValue(512);

                            b1.Property<string>("Model")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("TEXT");

                            b1.Property<string>("OllamaEndpoint")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("Parameters")
                                .HasColumnType("TEXT");

                            b1.Property<float>("Temperature")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("REAL")
                                .HasDefaultValue(0.7f);

                            b1.HasKey("AgentConfigId");

                            b1.ToTable("AgentConfigs");

                            b1.WithOwner()
                                .HasForeignKey("AgentConfigId");
                        });

                    b.OwnsOne("PjSip.App.Models.AgentConfig+WhisperConfig", "Whisper", b1 =>
                        {
                            b1.Property<int>("AgentConfigId")
                                .HasColumnType("INTEGER");

                            b1.Property<bool>("EnableTranslation")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Endpoint")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("Language")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(5)
                                .HasColumnType("TEXT")
                                .HasDefaultValue("en");

                            b1.Property<int>("Timeout")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER")
                                .HasDefaultValue(30);

                            b1.HasKey("AgentConfigId");

                            b1.ToTable("AgentConfigs");

                            b1.WithOwner()
                                .HasForeignKey("AgentConfigId");
                        });

                    b.Navigation("Auralis")
                        .IsRequired();

                    b.Navigation("LLM")
                        .IsRequired();

                    b.Navigation("Whisper")
                        .IsRequired();
                });

            modelBuilder.Entity("PjSip.App.Models.SipAccount", b =>
                {
                    b.HasOne("PjSip.App.Models.AgentConfig", "Agent")
                        .WithMany()
                        .HasForeignKey("AgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agent");
                });

            modelBuilder.Entity("PjSip.App.Models.SipCall", b =>
                {
                    b.HasOne("PjSip.App.Models.SipAccount", "Account")
                        .WithMany("Calls")
                        .HasForeignKey("SipAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("PjSip.App.Models.SipAccount", b =>
                {
                    b.Navigation("Calls");
                });
#pragma warning restore 612, 618
        }
    }
}
