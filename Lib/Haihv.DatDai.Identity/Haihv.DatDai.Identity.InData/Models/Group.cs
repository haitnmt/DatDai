using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Haihv.DatDai.Identity.InData.Models;

public class Group
{
    /// <summary>
    /// GUID của nhóm.
    /// </summary>
    [JsonPropertyName("id")]
    [Column("Id", TypeName = "varchar(36)")]
    [MaxLength(36)]
    public Guid Id { get; init; } = Guid.CreateVersion7();
}