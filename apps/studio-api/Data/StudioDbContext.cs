using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using studio_api.Models;

namespace studio_api.Data;

public class StudioDbContext(DbContextOptions<StudioDbContext> options) : DbContext(options)
{
    public DbSet<Moment> Moments => Set<Moment>();
    public DbSet<MomentImage> MomentImages => Set<MomentImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudioDbContext).Assembly);
    }
}

public class MomentConfiguration : IEntityTypeConfiguration<Moment>
{
    public void Configure(EntityTypeBuilder<Moment> builder)
    {
        builder.Property(m => m.Title).HasMaxLength(80).IsRequired();
        builder.Property(m => m.Subtitle).HasMaxLength(200);
        builder.Property(m => m.Destination).HasMaxLength(100);
        builder.Property(m => m.Vibe).HasMaxLength(100);

        builder.HasMany(m => m.Images)
            .WithOne(i => i.Moment)
            .HasForeignKey(i => i.MomentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class MomentImageConfiguration : IEntityTypeConfiguration<MomentImage>
{
    public void Configure(EntityTypeBuilder<MomentImage> builder)
    {
        builder.Property(mi => mi.Url).HasMaxLength(200).IsRequired();
        builder.Property(mi => mi.Caption).HasMaxLength(200);
    }
}