using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<File> Files { get; set; }

        public DbSet<Folder> Folders { get; set; }

        public DbSet<FileUser> FileUsers { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            FileOnModelCreating(builder);
            FileUserOnModelCreating(builder);

            base.OnModelCreating(builder);
        }

        private void FileOnModelCreating(ModelBuilder builder)
        {
            builder.Entity<File>()
                .HasKey(x => x.Id);

            builder.Entity<File>()
                .HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId);
        }

        private void FileUserOnModelCreating(ModelBuilder builder)
        {
            builder.Entity<FileUser>()
                .HasKey(x => x.Id);

            builder.Entity<FileUser>()
                .Property(x => x.FileId)
                .IsRequired();

            builder.Entity<FileUser>()
                .Property(x => x.UserId)
                .IsRequired();
        }
    }
}
