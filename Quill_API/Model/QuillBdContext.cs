using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Quill_API.Model;

public partial class QuillBdContext : DbContext
{

    public static QuillBdContext _context;
    public static QuillBdContext Context
    {
        get
        {
            if (_context == null)
                _context = new QuillBdContext();
            return _context;
        }
    }
    public QuillBdContext()
    {
    }

    public QuillBdContext(DbContextOptions<QuillBdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=1234;database=db_articles", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("articles");

            entity.HasIndex(e => e.AuthorId, "author_id");

            entity.HasIndex(e => e.IdTopics, "topics_idfk_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.IdTopics).HasColumnName("id_topics");
            entity.Property(e => e.PublishedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("published_at");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'active'")
                .HasColumnType("enum('active','deleted')")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .HasColumnName("title");

            entity.HasOne(d => d.Author).WithMany(p => p.Articles)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("articles_ibfk_1");

            entity.HasOne(d => d.IdTopicsNavigation).WithMany(p => p.Articles)
                .HasForeignKey(d => d.IdTopics)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("topics_idfk");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("comments");

            entity.HasIndex(e => e.ArticleId, "article_id");

            entity.HasIndex(e => e.AuthorId, "author_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.PublishedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("published_at");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'published'")
                .HasColumnType("enum('published','deleted')")
                .HasColumnName("status");

            entity.HasOne(d => d.Article).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comments_ibfk_1");

            entity.HasOne(d => d.Author).WithMany(p => p.Comments)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comments_ibfk_2");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ratings");

            entity.HasIndex(e => new { e.ArticleId, e.UserId }, "article_id").IsUnique();

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.Rating1).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Article).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ratings_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ratings_ibfk_2");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.IdTopics).HasName("PRIMARY");

            entity.ToTable("topics");

            entity.Property(e => e.IdTopics).HasColumnName("id_topics");
            entity.Property(e => e.NameTopics)
                .HasMaxLength(45)
                .HasColumnName("name_topics");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.Nickname, "nickname").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Nickname)
                .HasMaxLength(100)
                .HasColumnName("nickname");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'user'")
                .HasColumnType("enum('user','admin')")
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'active'")
                .HasColumnType("enum('active','deleted')")
                .HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
