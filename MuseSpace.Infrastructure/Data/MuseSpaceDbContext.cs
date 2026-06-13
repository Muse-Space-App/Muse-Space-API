using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;

namespace MuseSpace.Infrastructure.Data;

public sealed class MuseSpaceDbContext : DbContext
{
    public MuseSpaceDbContext(DbContextOptions<MuseSpaceDbContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<Otp> Otps { get; set; } = null!;
    public DbSet<Artwork> Artwork { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<ArtworkTag> ArtworkTags { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Like> Likes { get; set; } = null!;
    public DbSet<Bookmark> Bookmarks { get; set; } = null!;
    public DbSet<Share> Shares { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<GroupMember> GroupMembers { get; set; } = null!;
    public DbSet<GroupPost> GroupPosts { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<EventRsvp> EventRsvps { get; set; } = null!;
    public DbSet<Follow> Follows { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<Commission> Commissions { get; set; } = null!;
    public DbSet<CommissionMessage> CommissionMessages { get; set; } = null!;
    public DbSet<GroupPostLike> GroupPostLikes { get; set; } = null!;
    public DbSet<GroupPostComment> GroupPostComments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasMany(e => e.Users).WithOne(u => u.Role).HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
            entity.Property(e => e.BanReason).HasMaxLength(500);
            entity.HasOne(e => e.Role).WithMany(r => r.Users).HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UserProfile).WithOne(up => up.User).HasForeignKey<UserProfile>(up => up.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.CreatedArtwork).WithOne(a => a.Creator).HasForeignKey(a => a.CreatorId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Comments).WithOne(c => c.User).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Likes).WithOne(l => l.User).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Followers).WithOne(f => f.Follower).HasForeignKey(f => f.FollowerId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Following).WithOne(f => f.FollowingUser).HasForeignKey(f => f.FollowingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Reports).WithOne(r => r.ReportedBy).HasForeignKey(r => r.ReportedById).OnDelete(DeleteBehavior.NoAction);
            entity.HasMany(e => e.ReceivedReports).WithOne(r => r.TargetUser).HasForeignKey(r => r.TargetUserId).OnDelete(DeleteBehavior.NoAction);
            entity.HasMany(e => e.Notifications).WithOne(n => n.User).HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.RequestedCommissions).WithOne(c => c.Requester).HasForeignKey(c => c.RequesterId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.ReceivedCommissions).WithOne(c => c.Artist).HasForeignKey(c => c.ArtistId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.CommissionMessages).WithOne(cm => cm.Sender).HasForeignKey(cm => cm.SenderId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.GroupPostLikes).WithOne(l => l.User).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.GroupPostComments).WithOne(c => c.User).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Otp>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.Purpose, e.IsUsed });
            entity.HasIndex(e => e.ExpiresAtUtc);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Purpose).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.BannerUrl).HasMaxLength(500);
            entity.Property(e => e.SocialLinks).HasMaxLength(1000); // JSON
            entity.Property(e => e.CreatorTier).HasMaxLength(50);
            entity.HasOne(e => e.User).WithOne(u => u.UserProfile).HasForeignKey<UserProfile>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Artwork>(entity =>
        {
            entity.ToTable("Artwork");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CreatorId);
            entity.HasIndex(e => e.CreatedAtUtc);
            entity.HasIndex(e => e.IsApproved);
            entity.HasIndex(e => e.IsSoftDeleted);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(5000);
            entity.Property(e => e.ContentUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ThumbnailUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.MediaType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DeleteReason).HasMaxLength(500);
            entity.HasOne(e => e.Creator).WithMany(u => u.CreatedArtwork).HasForeignKey(e => e.CreatorId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.ArtworkTags).WithOne(at => at.Artwork).HasForeignKey(at => at.ArtworkId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Comments).WithOne(c => c.Artwork).HasForeignKey(c => c.ArtworkId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Likes).WithOne(l => l.Artwork).HasForeignKey(l => l.ArtworkId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Reports).WithOne(r => r.Artwork).HasForeignKey(r => r.ArtworkId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.UsageCount);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.HasMany(e => e.ArtworkTags).WithOne(at => at.Tag).HasForeignKey(at => at.TagId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ArtworkTag>(entity =>
        {
            entity.HasKey(at => new { at.ArtworkId, at.TagId });
            entity.HasOne(at => at.Artwork).WithMany(a => a.ArtworkTags).HasForeignKey(at => at.ArtworkId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(at => at.Tag).WithMany(t => t.ArtworkTags).HasForeignKey(at => at.TagId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ArtworkId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAtUtc);
            entity.HasIndex(e => e.IsSoftDeleted);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(5000);
            entity.HasOne(e => e.Artwork).WithMany(a => a.Comments).HasForeignKey(e => e.ArtworkId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany(u => u.Comments).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.ParentComment).WithMany(c => c.Replies).HasForeignKey(e => e.ParentCommentId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(l => new { l.UserId, l.ArtworkId });
            entity.HasIndex(l => l.CreatedAtUtc);
            entity.HasOne(l => l.User).WithMany(u => u.Likes).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(l => l.Artwork).WithMany(a => a.Likes).HasForeignKey(l => l.ArtworkId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.HasKey(b => new { b.UserId, b.ArtworkId });
            entity.HasIndex(b => b.CreatedAtUtc);
            entity.HasOne(b => b.User).WithMany(u => u.Bookmarks).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(b => b.Artwork).WithMany(a => a.Bookmarks).HasForeignKey(b => b.ArtworkId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Share>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => s.UserId);
            entity.HasIndex(s => s.ArtworkId);
            entity.HasIndex(s => s.CreatedAtUtc);
            entity.Property(s => s.Platform).HasMaxLength(50);
            entity.HasOne(s => s.User).WithMany(u => u.Shares).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(s => s.Artwork).WithMany(a => a.Shares).HasForeignKey(s => s.ArtworkId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(f => new { f.FollowerId, f.FollowingId });
            entity.HasIndex(f => f.CreatedAtUtc);
            entity.HasOne(f => f.Follower).WithMany(u => u.Followers).HasForeignKey(f => f.FollowerId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(f => f.FollowingUser).WithMany(u => u.Following).HasForeignKey(f => f.FollowingId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.HasIndex(g => g.Name).IsUnique();
            entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
            entity.Property(g => g.Description).HasMaxLength(1000);
            entity.HasOne(g => g.Creator).WithMany(u => u.CreatedGroups).HasForeignKey(g => g.CreatorId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(gm => new { gm.GroupId, gm.UserId });
            entity.HasIndex(gm => gm.JoinedAtUtc);
            entity.HasOne(gm => gm.Group).WithMany(g => g.Members).HasForeignKey(gm => gm.GroupId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(gm => gm.User).WithMany(u => u.GroupMemberships).HasForeignKey(gm => gm.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GroupPost>(entity =>
        {
            entity.HasKey(gp => gp.Id);
            entity.HasIndex(gp => gp.GroupId);
            entity.HasIndex(gp => gp.CreatedAtUtc);
            entity.Property(gp => gp.Content).IsRequired().HasMaxLength(5000);
            entity.HasOne(gp => gp.Group).WithMany(g => g.Posts).HasForeignKey(gp => gp.GroupId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(gp => gp.Author).WithMany(u => u.GroupPosts).HasForeignKey(gp => gp.AuthorId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GroupPostLike>(entity =>
        {
            entity.HasKey(l => new { l.UserId, l.GroupPostId });
            entity.HasIndex(l => l.CreatedAtUtc);
            entity.HasOne(l => l.User).WithMany(u => u.GroupPostLikes).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(l => l.GroupPost).WithMany(gp => gp.Likes).HasForeignKey(l => l.GroupPostId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GroupPostComment>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.GroupPostId);
            entity.HasIndex(c => c.UserId);
            entity.HasIndex(c => c.CreatedAtUtc);
            entity.HasIndex(c => c.IsSoftDeleted);
            entity.Property(c => c.Content).IsRequired().HasMaxLength(5000);
            entity.HasOne(c => c.GroupPost).WithMany(gp => gp.Comments).HasForeignKey(c => c.GroupPostId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(c => c.User).WithMany(u => u.GroupPostComments).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.StartDateUtc);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.EventUrl).HasMaxLength(500);
            entity.HasOne(e => e.Organizer).WithMany(u => u.OrganizedEvents).HasForeignKey(e => e.OrganizerId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EventRsvp>(entity =>
        {
            entity.HasKey(er => new { er.EventId, er.UserId });
            entity.HasIndex(er => er.RsvpedAtUtc);
            entity.HasOne(er => er.Event).WithMany(e => e.Rsvps).HasForeignKey(er => er.EventId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(er => er.User).WithMany(u => u.EventRsvps).HasForeignKey(er => er.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ReportedById);
            entity.HasIndex(e => e.ArtworkId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAtUtc);
            entity.Property(e => e.ReportType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.AdminNotes).HasMaxLength(2000);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.HasOne(e => e.ReportedBy).WithMany(u => u.Reports).HasForeignKey(e => e.ReportedById).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Artwork).WithMany(a => a.Reports).HasForeignKey(e => e.ArtworkId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.ReviewedByAdmin).WithMany().HasForeignKey(e => e.ReviewedByAdminId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.EntityType);
            entity.HasIndex(e => e.CreatedAtUtc);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Changes).HasMaxLength(5000); // JSON
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsRead);
            entity.HasIndex(e => e.CreatedAtUtc);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ActionUrl).HasMaxLength(500);
            entity.HasOne(e => e.User).WithMany(u => u.Notifications).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.RelatedUser).WithMany().HasForeignKey(e => e.RelatedUserId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.RelatedArtwork).WithMany().HasForeignKey(e => e.RelatedArtworkId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Commission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.RequesterId);
            entity.HasIndex(e => e.ArtistId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAtUtc);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Requester).WithMany(u => u.RequestedCommissions).HasForeignKey(e => e.RequesterId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Artist).WithMany(u => u.ReceivedCommissions).HasForeignKey(e => e.ArtistId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CommissionMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CommissionId);
            entity.HasIndex(e => e.CreatedAtUtc);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            entity.HasOne(e => e.Commission).WithMany(c => c.Messages).HasForeignKey(e => e.CommissionId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Sender).WithMany(u => u.CommissionMessages).HasForeignKey(e => e.SenderId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
