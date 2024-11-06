namespace Haihv.DatDai.Identity.InData.Models;

public class AppRoleRegistration : RoleRegistration
{
    /// <summary>
    /// Mã định danh của vai trò.
    /// </summary>
    public Guid RoleId { get; set; }
}
