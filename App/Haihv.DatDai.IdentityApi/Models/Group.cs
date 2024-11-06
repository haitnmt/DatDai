using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Haihv.DatDai.IdentityApi.Interface;

namespace Haihv.DatDai.IdentityApi.Models;

public class Group : LdapGroup, IGroup
{
    /// <summary>
    /// GUID của nhóm.
    /// </summary>
    [JsonPropertyName("id")]
    [Column("Id", TypeName = "varchar(36)")]
    [MaxLength(36)]
    public override Guid Id { get; init; } = Guid.CreateVersion7();
}