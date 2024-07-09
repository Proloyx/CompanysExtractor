using System;
using System.Collections.Generic;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace Extractor.Models.DBPostgresSQL;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Ticker> Tickers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql(Env.GetString("DbConnection"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Cik).HasName("companies_pkey");

            entity.ToTable("companies", "fso");

            entity.Property(e => e.Cik)
                .HasMaxLength(50)
                .HasColumnName("cik");
            entity.Property(e => e.Entityname)
                .HasMaxLength(100)
                .HasColumnName("entityname");
        });

        modelBuilder.Entity<Ticker>(entity =>
        {
            entity.HasKey(e => e.CikStr).HasName("tickers_pkey");

            entity.ToTable("tickers", "fso");

            entity.Property(e => e.CikStr)
                .HasMaxLength(50)
                .HasColumnName("cik_str");
            entity.Property(e => e.Ticker1)
                .HasMaxLength(50)
                .HasColumnName("ticker");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
