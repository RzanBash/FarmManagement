using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using FarmManagement.Models;

namespace FarmManagement.Data
{           
    public partial class FarmBookingDBContext : DbContext
    {
        public FarmBookingDBContext()
        {
        }

        public FarmBookingDBContext(DbContextOptions<FarmBookingDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Cabin> Cabins { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("server=.; database=FarmBookingDB; trusted_connection=true; ");
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(e => e.CheckInDate).HasColumnType("datetime");

                entity.Property(e => e.CheckOutDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerName).HasMaxLength(100);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'Pending')");

                entity.HasOne(d => d.Cabin)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CabinId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bookings_Cabins");
            });

            modelBuilder.Entity<Cabin>(entity =>
            {
                entity.Property(e => e.ImageUrl).HasMaxLength(255);

                entity.Property(e => e.IsAvailable).HasDefaultValueSql("((1))");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.PricePerNight).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ImageUrl).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.Username, "IX_Users")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.PasswordHash).HasMaxLength(255);

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('Admin')");

                entity.Property(e => e.Username).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
