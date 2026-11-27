using GameRanking.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GameRanking.Web.Data;

public sealed class GameRankingDbContext(DbContextOptions<GameRankingDbContext> options)
    : IdentityUserContext<IdentityUser>(options)
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Match> Matches => Set<Match>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
    {
        configuration.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<IdentityUser>().HasIndex(user => user.NormalizedEmail)
            .HasDatabaseName("EmailIndex").IsUnique();

        builder.Entity<Player>(player =>
        {
            player.ToTable("Players", table =>
            {
                table.HasCheckConstraint("CK_Players_Stats", "[TotalScore] >= 0 AND [TotalExp] >= 0 AND [Wins] >= 0 AND [Losses] >= 0");
                table.HasCheckConstraint("CK_Players_DisplayName", "LEN(TRIM([DisplayName])) > 0");
            });
            player.Property(item => item.DisplayName).HasMaxLength(80);
            player.Property(item => item.TotalScore).HasDefaultValue(0);
            player.Property(item => item.TotalExp).HasDefaultValue(0);
            player.Property(item => item.Wins).HasDefaultValue(0);
            player.Property(item => item.Losses).HasDefaultValue(0);
            player.Property(item => item.RowVersion).IsRowVersion();
            player.HasOne<IdentityUser>().WithOne().HasForeignKey<Player>(item => item.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Match>(match =>
        {
            match.ToTable("Matches", table =>
                table.HasCheckConstraint("CK_Matches_DistinctPlayers", "[WinnerPlayerId] <> [LoserPlayerId]"));
            match.HasOne<Player>().WithMany().HasForeignKey(item => item.WinnerPlayerId)
                .OnDelete(DeleteBehavior.Restrict);
            match.HasOne<Player>().WithMany().HasForeignKey(item => item.LoserPlayerId)
                .OnDelete(DeleteBehavior.Restrict);
            match.HasIndex(item => new { item.WinnerPlayerId, item.PlayedAtUtc });
            match.HasIndex(item => new { item.LoserPlayerId, item.PlayedAtUtc });
        });
    }

    private sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
        value => value,
        value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
}
