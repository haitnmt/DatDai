using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Haihv.DatDai.Lib.Identity.Data.Entities;

public abstract class BaseFromLdap : BaseEntry
{
    /// <summary>
    /// Tên phân biệt (Distinguished Name) của người đối tượng LDAP.
    /// </summary>
    [Column("DistinguishedName", TypeName = "varchar(150)")]
    [MaxLength(150)]
    [JsonPropertyName("distinguishedName")]
    public string? DistinguishedName { get; set; }
}