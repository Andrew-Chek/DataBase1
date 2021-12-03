using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace lab3
{
    public partial class postgresContext : DbContext
    {
        public postgresContext()
        {
        }

        public postgresContext(DbContextOptions<postgresContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrdersItem> OrdersItems { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=2003Lipovetc");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pg_catalog", "adminpack");

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("items");

                entity.Property(e => e.ItemId)
                    .HasColumnName("item_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Availability).HasColumnName("availability");

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrdId)
                    .HasName("orders_pkey");

                entity.ToTable("orders");

                entity.Property(e => e.OrdId)
                    .HasColumnName("ord_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.UsId).HasColumnName("us_id");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("us_id");
            });

            modelBuilder.Entity<OrdersItem>(entity =>
            {
                entity.ToTable("orders_items");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.OrdId).HasColumnName("ord_id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UsId)
                    .HasName("users_pkey");

                entity.ToTable("users");

                entity.Property(e => e.UsId)
                    .HasColumnName("us_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(120)
                    .HasColumnName("password");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
