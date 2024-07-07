﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Extractor.Models.DBSQLite;

public partial class SQLiteDbContext : DbContext
{
    public SQLiteDbContext()
    {
    }

    public SQLiteDbContext(DbContextOptions<SQLiteDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=FSObserver.db3");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
             entity.HasKey(e => e.CIK).HasName("Company_Id");
            entity.Property(e => e.CIK).HasColumnName("Company_Id");
            entity.Property(e => e.entityName).HasColumnName("Company");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
