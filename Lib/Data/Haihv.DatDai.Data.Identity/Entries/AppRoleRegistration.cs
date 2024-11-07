namespace Haihv.DatDai.Data.Identity.Entries;

public class AppRoleRegistration : RoleRegistration
{
    /// <summary>
    /// Mã định danh của vai trò.
    /// </summary>
    public Guid RoleId { get; set; }
}
