using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Identity.Data.Entries;

[PrimaryKey("Id")]
public class Group : BaseFromLdap
{
    /// <summary>
    /// Tên chung (Common Name) của nhóm.
    /// </summary>
    [JsonPropertyName("groupName")]
    [Column("GroupName", TypeName = "varchar(50)")]
    [MaxLength(50)]
    public string GroupName { get; set; } = string.Empty;
    /// <summary>
    /// Tập hợp các nhóm mà nhóm này là thành viên.
    /// </summary>
    [JsonPropertyName("memberOf")]
    [Column("MemberOf", TypeName = "uuid[]")]
    public List<Guid> MemberOf { get; set; } = [];
    
    /// <summary>
    /// Kiểu xác thực của người dùng.
    /// </summary>
    /// <remarks>
    /// <c>0: CSDL/SystemUser </c>
    /// <c>1: ADDC/LDAP</c>
    /// </remarks>
    [JsonPropertyName("groupType")]
    [Column("GroupType", TypeName = "integer")]
    public int GroupType { get; set; }
}