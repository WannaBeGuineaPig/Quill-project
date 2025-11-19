using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using TEST.Model;

namespace TEST;

public partial class ExamProctoringSuiteContext : DbContext
{
    public ExamProctoringSuiteContext()
    {
    }

    public ExamProctoringSuiteContext(DbContextOptions<ExamProctoringSuiteContext> options)
        : base(options)
    {
    }

    private static ExamProctoringSuiteContext _context;

    public static ExamProctoringSuiteContext GetContext
    {
        get
        {
            if (_context == null)
            {
                _context = new ExamProctoringSuiteContext();
            }
            return _context;
        }
    }

    public virtual DbSet<FavoriteTest> FavoriteTests { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionAnswer> QuestionAnswers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TestCategory> TestCategories { get; set; }

    public virtual DbSet<TestQuestion> TestQuestions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAnswer> UserAnswers { get; set; }

    public virtual DbSet<UserTestResult> UserTestResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=5225;database=exam_proctoring_suite", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<FavoriteTest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("favorite_test")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TestId, "test_key_idx");

            entity.HasIndex(e => e.UserId, "user_key_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Test).WithMany(p => p.FavoriteTests)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("test_key");

            entity.HasOne(d => d.User).WithMany(p => p.FavoriteTests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_key");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("questions")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CreatedBy, "fk_questions_created_by");

            entity.HasIndex(e => e.TestId, "fk_questions_test_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Points)
                .HasDefaultValueSql("'1'")
                .HasColumnName("points");
            entity.Property(e => e.QuestionText)
                .HasColumnType("text")
                .HasColumnName("question_text");
            entity.Property(e => e.QuestionType)
                .HasColumnType("enum('single_choice','multiple_choice','open_answer')")
                .HasColumnName("question_type");
            entity.Property(e => e.TestId).HasColumnName("test_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_questions_created_by");

            entity.HasOne(d => d.Test).WithMany(p => p.Questions)
                .HasForeignKey(d => d.TestId)
                .HasConstraintName("fk_questions_test_id");
        });

        modelBuilder.Entity<QuestionAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("question_answers")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.QuestionId, "fk_question_answers_question_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerText)
                .HasMaxLength(255)
                .HasColumnName("answer_text");
            entity.Property(e => e.IsCorrect)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_correct");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("fk_question_answers_question_id");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("roles")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tests")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CategoryId, "fk_tests_category_id");

            entity.HasIndex(e => e.CreatedBy, "fk_tests_created_by");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.PassingScore)
                .HasDefaultValueSql("'70'")
                .HasColumnName("passing_score");
            entity.Property(e => e.TimeLimitMinutes).HasColumnName("time_limit_minutes");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Category).WithMany(p => p.Tests)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tests_category_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Tests)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tests_created_by");
        });

        modelBuilder.Entity<TestCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("test_categories")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CreatedBy, "fk_test_categories_created_by");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TestCategories)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_test_categories_created_by");
        });

        modelBuilder.Entity<TestQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("test_questions")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.QuestionId, "fk_test_questions_question_id");

            entity.HasIndex(e => new { e.TestId, e.QuestionId }, "unique_test_question").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Points)
                .HasDefaultValueSql("'1'")
                .HasColumnName("points");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.TestId).HasColumnName("test_id");

            entity.HasOne(d => d.Question).WithMany(p => p.TestQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_test_questions_question_id");

            entity.HasOne(d => d.Test).WithMany(p => p.TestQuestions)
                .HasForeignKey(d => d.TestId)
                .HasConstraintName("fk_test_questions_test_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("users")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.RoleId, "fk_users_role_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(100)
                .HasColumnName("patronymic");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users_role_id");
        });

        modelBuilder.Entity<UserAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("user_answers")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.QuestionId, "fk_user_answers_question_id");

            entity.HasIndex(e => e.TestResultId, "fk_user_answers_test_result_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerText)
                .HasColumnType("text")
                .HasColumnName("answer_text");
            entity.Property(e => e.IsCorrect)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_correct");
            entity.Property(e => e.PointsEarned)
                .HasDefaultValueSql("'0'")
                .HasColumnName("points_earned");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.TestResultId).HasColumnName("test_result_id");

            entity.HasOne(d => d.Question).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_answers_question_id");

            entity.HasOne(d => d.TestResult).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.TestResultId)
                .HasConstraintName("fk_user_answers_test_result_id");
        });

        modelBuilder.Entity<UserTestResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("user_test_results")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TestId, "fk_user_test_results_test_id");

            entity.HasIndex(e => e.UserId, "fk_user_test_results_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FinishedAt)
                .HasColumnType("datetime")
                .HasColumnName("finished_at");
            entity.Property(e => e.IsPassed)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_passed");
            entity.Property(e => e.MaxScore).HasColumnName("max_score");
            entity.Property(e => e.ScoreAchieved)
                .HasDefaultValueSql("'0'")
                .HasColumnName("score_achieved");
            entity.Property(e => e.StartedAt)
                .HasColumnType("datetime")
                .HasColumnName("started_at");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Test).WithMany(p => p.UserTestResults)
                .HasForeignKey(d => d.TestId)
                .HasConstraintName("fk_user_test_results_test_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserTestResults)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_test_results_user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
