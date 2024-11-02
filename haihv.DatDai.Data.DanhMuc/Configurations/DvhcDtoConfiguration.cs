using haihv.DatDai.Data.DanhMuc.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace haihv.DatDai.Data.DanhMuc.Configurations;

public class DvhcDtoConfiguration :IEntityTypeConfiguration<DvhcDto>
{
    public void Configure(EntityTypeBuilder<DvhcDto> builder)
    {
        builder.Property(e => e.Id)
            .HasConversion(
                    v => v.ToString(), // Chuyển đổi từ Ulid sang string
                    v => Ulid.Parse(v) // Chuyển đổi từ string sang Ulid
                );
        builder.HasKey(e => e.Id);
        builder.Property(e => e.MaKyHieu).IsRequired();
        builder.Property(e => e.TenGiaTri).IsRequired();
        builder.Property(e => e.MaXa).HasMaxLength(5);
    }
}