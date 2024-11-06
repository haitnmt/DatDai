namespace Haihv.DatDai.IdentityApi.Interface;

public interface IGroup : ILdapGroup
{
    string? GhiChu { get; set; }
}