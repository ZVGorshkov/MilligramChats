using MilligramServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using File = MilligramServer.Domain.Entities.File;
using MilligramServer.Common;

namespace MilligramServer.Database.Context;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UsersRoles => Set<UserRole>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<UserChat> UsersChats => Set<UserChat>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<File> Files => Set<File>();

    private readonly ILogger<ApplicationContext>? _logger;

    public ApplicationContext(
        DbContextOptions<ApplicationContext> dbContextOptions,
        ILogger<ApplicationContext>? logger = null)
        : base(dbContextOptions)
    {
        _logger = logger;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.LogTo(LogMessage, LogLevel.Information);
    }

    private void LogMessage(string message)
    {
        _logger?.LogInformation(message);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region User

        modelBuilder.Entity<User>()
            .Property(u => u.Name)
            .HasMaxLength(Constants.MaxUserNameLength);

        modelBuilder.Entity<User>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Users_Name",
                "LEN(Name) > 0"));

        modelBuilder.Entity<User>()
            .HasIndex(u => u.NormalizedName)
            .IsUnique();

        modelBuilder.Entity<User>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Users_NormalizedName",
                "LEN(NormalizedName) > 0"));

        modelBuilder.Entity<User>()
            .Property(u => u.Nickname)
            .HasMaxLength(Constants.MaxUserNicknameLength);

        modelBuilder.Entity<User>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Users_Nickname",
                "LEN(Nickname) > 0"));

        modelBuilder.Entity<User>()
             .Property(u => u.PasswordHash)
             .HasAnnotation("MinLength", Constants.MinUserPasswordLength);

        modelBuilder.Entity<User>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Users_PasswordHash",
                "LEN(PasswordHash) >= 8"
            ));

        #endregion

        #region Role

        modelBuilder.Entity<Role>()
            .Property(r => r.Name)
            .HasMaxLength(Constants.MaxRoleNameLength);

        modelBuilder.Entity<Role>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Roles_Name",
                "LEN(Name) > 0"));

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.NormalizedName)
            .IsUnique();

        modelBuilder.Entity<Role>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Roles_NormalizedName",
                "LEN(NormalizedName) > 0"));

        #endregion

        #region UserRole

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UsersRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UsersRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        #endregion

        #region Chat

        modelBuilder.Entity<Chat>()
            .HasOne(c => c.OwnerUser)
            .WithMany(u => u.ChatsOwner)
            .HasForeignKey(c => c.OwnerUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Chat>()
           .Property(c => c.Name)
           .HasMaxLength(Constants.MaxChatNameLength);

        modelBuilder.Entity<Chat>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Chats_Name",
                "LEN(Name) > 0"));

        #endregion

        #region UserChat

        modelBuilder.Entity<UserChat>()
           .HasKey(uc => new { uc.UserId, uc.ChatId });

        modelBuilder.Entity<UserChat>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.UsersChats)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserChat>()
            .HasOne(uc => uc.Chat)
            .WithMany(c => c.UsersChats)
            .HasForeignKey(uc => uc.ChatId)
            .OnDelete(DeleteBehavior.NoAction);

        #endregion

        #region Contact

        modelBuilder.Entity<Contact>()
            .HasOne(c => c.OwnerUser)
            .WithMany(u => u.OwnerUserForContacts)
            .HasForeignKey(c => c.OwnerUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Contact>()
            .HasOne(c => c.AddedUser)
            .WithMany(u => u.AddedUserForContacts)
            .HasForeignKey(c => c.AddedUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Contact>()
            .Property(c => c.Name)
            .HasMaxLength(Constants.MaxContactNameLength);

        modelBuilder.Entity<Contact>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Contacts_Name",
                "Name IS NULL OR LEN(Name) > 0"));

        #endregion

        #region Message

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.File)
            .WithOne(f => f.Message)
            .HasForeignKey<Message>(m => m.FileId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Message>()
            .Property(m => m.CreationTime)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<Message>()
            .Property(m => m.LastChangeTime)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<Message>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Messages_Text",
                "Text IS NULL OR LEN(Text) > 0"));

        #endregion

        #region File

        modelBuilder.Entity<File>()
            .Property(f => f.Name)
            .HasMaxLength(Constants.MaxFileNameLength);

        modelBuilder.Entity<File>()
            .Property(f => f.Extension)
            .HasMaxLength(Constants.MaxFileExtensionLength);

        modelBuilder.Entity<File>()
            .ToTable(t => t.HasCheckConstraint(
            "CK_Files_Name",
            "LEN(Name) > 0"));

        modelBuilder.Entity<File>()
            .ToTable(t => t.HasCheckConstraint(
            "CK_Files_Extension",
            "LEN(Extension) > 0"));

        modelBuilder.Entity<File>()
            .ToTable(t => t.HasCheckConstraint(
            "CK_Files_SizeBytes",
            $"SizeBytes > 0 AND SizeBytes <= {Constants.MaxFileSizeBytes}"));

        #endregion
    }
}