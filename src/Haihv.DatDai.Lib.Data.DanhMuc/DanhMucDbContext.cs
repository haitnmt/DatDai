using Audit.Core;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.DanhMuc;

public class DanhMucDbContext(string connectionString, AuditDataProvider? auditDataProvider)
    : BaseDbContext(connectionString, auditDataProvider)
{
    public DbSet<Dvhc> Dvhc { get; set; } = default!;
    public DbSet<DanToc> DanToc { get; set; } = default!;
    public DbSet<QuocTich> QuocTich { get; set; } = default!;

}