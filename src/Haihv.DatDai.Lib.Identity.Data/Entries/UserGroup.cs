using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Haihv.DatDai.Lib.Data.Base;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Identity.Data.Entries;
[PrimaryKey("Id")]
public class UserGroup : BaseEntry
{
    /// <summary>
    /// GUID của người dùng.
    /// </summary>
    [Column("UserId", TypeName = "uuid")]
    public Guid UserId { get; set; } = Guid.Empty;
    /// <summary>
    /// GUID của nhóm.
    /// </summary>
    [Column("GroupId", TypeName = "uuid")]
    public Guid GroupId { get; set; } = Guid.Empty;
}