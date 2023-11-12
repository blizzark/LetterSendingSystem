using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebServerMail;

public partial class MailDbContext : DbContext
{
    public MailDbContext()
    {
    }

    public MailDbContext(DbContextOptions<MailDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Letter> Letters { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Letter>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Recipient)
                .HasDefaultValueSql("((1))")
                .HasColumnName("recipient");
            entity.Property(e => e.Sender)
                .HasDefaultValueSql("((1))")
                .HasColumnName("sender");
            entity.Property(e => e.Text)
                .HasColumnType("text")
                .HasColumnName("text");
            entity.Property(e => e.Titel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("titel");

            entity.HasOne(d => d.RecipientNavigation).WithMany(p => p.LetterRecipientNavigations)
                .HasForeignKey(d => d.Recipient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Letters_Users_Recipient");

            entity.HasOne(d => d.SenderNavigation).WithMany(p => p.LetterSenderNavigations)
                .HasForeignKey(d => d.Sender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Letters_Users_Sender");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("firstName");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.SecondName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("secondName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
