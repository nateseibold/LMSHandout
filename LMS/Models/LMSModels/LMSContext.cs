using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class LMSContext : DbContext
    {
        public LMSContext()
        {
        }

        public LMSContext(DbContextOptions<LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<AssignmentCat> AssignmentCats { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Enrolled> Enrolleds { get; set; } = null!;
        public virtual DbSet<Professor> Professors { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Submission> Submissions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("name=LMS:LMSConnectionString", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.3-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("latin1_swedish_ci")
                .HasCharSet("latin1");

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.SId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.UId, "uID")
                    .IsUnique();

                entity.Property(e => e.SId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("sID");

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FName)
                    .HasMaxLength(100)
                    .HasColumnName("fName");

                entity.Property(e => e.LName)
                    .HasMaxLength(100)
                    .HasColumnName("lName");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.AssId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.Cat, e.Name }, "cat")
                    .IsUnique();

                entity.Property(e => e.AssId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("assID");

                entity.Property(e => e.Cat)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("cat");

                entity.Property(e => e.Content)
                    .HasMaxLength(8192)
                    .HasColumnName("content");

                entity.Property(e => e.DueDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dueDate");

                entity.Property(e => e.MaxPoint).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasOne(d => d.CatNavigation)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.Cat)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Assignments_ibfk_1");
            });

            modelBuilder.Entity<AssignmentCat>(entity =>
            {
                entity.HasKey(e => e.AssId)
                    .HasName("PRIMARY");

                entity.ToTable("AssignmentCat");

                entity.HasIndex(e => new { e.Name, e.Class }, "Name")
                    .IsUnique();

                entity.HasIndex(e => e.Class, "class");

                entity.Property(e => e.AssId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("AssID");

                entity.Property(e => e.Class)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("class");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Weight).HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.ClassNavigation)
                    .WithMany(p => p.AssignmentCats)
                    .HasForeignKey(d => d.Class)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCat_ibfk_1");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasIndex(e => new { e.Course, e.SYear, e.Season }, "course")
                    .IsUnique();

                entity.HasIndex(e => e.Professor, "professor");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.Course)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("course");

                entity.Property(e => e.EndTime)
                    .HasColumnType("time")
                    .HasColumnName("endTime");

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .HasColumnName("location");

                entity.Property(e => e.Professor)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("professor");

                entity.Property(e => e.SYear)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("sYear");

                entity.Property(e => e.Season)
                    .HasMaxLength(6)
                    .HasColumnName("season");

                entity.Property(e => e.StartTime)
                    .HasColumnType("time")
                    .HasColumnName("startTime");

                entity.HasOne(d => d.CourseNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.Course)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_2");

                entity.HasOne(d => d.ProfessorNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.Professor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_1");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.Subject, e.Number }, "subject")
                    .IsUnique();

                entity.Property(e => e.CId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("cID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Number)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("number");

                entity.Property(e => e.Subject)
                    .HasMaxLength(4)
                    .HasColumnName("subject");

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Subject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Subject)
                    .HasName("PRIMARY");

                entity.Property(e => e.Subject)
                    .HasMaxLength(4)
                    .HasColumnName("subject");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Enrolled>(entity =>
            {
                entity.ToTable("Enrolled");

                entity.HasIndex(e => e.Class, "class");

                entity.HasIndex(e => new { e.Student, e.Class }, "student")
                    .IsUnique();

                entity.Property(e => e.EnrolledId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("enrolledID");

                entity.Property(e => e.Class)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("class");

                entity.Property(e => e.Grade)
                    .HasMaxLength(2)
                    .HasColumnName("grade");

                entity.Property(e => e.Student)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("student");

                entity.HasOne(d => d.ClassNavigation)
                    .WithMany(p => p.Enrolleds)
                    .HasForeignKey(d => d.Class)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrolled_ibfk_2");

                entity.HasOne(d => d.StudentNavigation)
                    .WithMany(p => p.Enrolleds)
                    .HasForeignKey(d => d.Student)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrolled_ibfk_1");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.SId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Subject, "subject");

                entity.HasIndex(e => e.UId, "uID")
                    .IsUnique();

                entity.Property(e => e.SId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("sID");

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FName)
                    .HasMaxLength(100)
                    .HasColumnName("fName");

                entity.Property(e => e.LName)
                    .HasMaxLength(100)
                    .HasColumnName("lName");

                entity.Property(e => e.Subject)
                    .HasMaxLength(4)
                    .HasColumnName("subject");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.Subject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.SId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Subject, "subject");

                entity.HasIndex(e => e.UId, "uID")
                    .IsUnique();

                entity.Property(e => e.SId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("sID");

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FName)
                    .HasMaxLength(100)
                    .HasColumnName("fName");

                entity.Property(e => e.LName)
                    .HasMaxLength(100)
                    .HasColumnName("lName");

                entity.Property(e => e.Subject)
                    .HasMaxLength(4)
                    .HasColumnName("subject");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Subject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_1");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => e.SubId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Assigment, "assigment");

                entity.HasIndex(e => new { e.Student, e.Assigment }, "student")
                    .IsUnique();

                entity.Property(e => e.SubId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("subID");

                entity.Property(e => e.Assigment)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("assigment");

                entity.Property(e => e.Contents)
                    .HasMaxLength(8192)
                    .HasColumnName("contents");

                entity.Property(e => e.Score)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("score");

                entity.Property(e => e.Student)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("student");

                entity.Property(e => e.SubmitDate)
                    .HasColumnType("datetime")
                    .HasColumnName("submitDate");

                entity.HasOne(d => d.AssigmentNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.Assigment)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_2");

                entity.HasOne(d => d.StudentNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.Student)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
