namespace Haihv.DatDai.Lib.Identity.Ldap.Entries;

/// <summary>
/// Đại diện cho một nhóm LDAP.
/// </summary>
public class GroupLdap : BaseLdap
{
    /// <summary>
    /// Danh sách các nhóm là thành viên.
    /// </summary>
    public HashSet<string> GroupMembers { get; set; } = [];
}