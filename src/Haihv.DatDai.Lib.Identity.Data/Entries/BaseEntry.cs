using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Haihv.DatDai.Lib.Data.Base;

namespace Haihv.DatDai.Lib.Identity.Data.Entries;

public abstract class BaseEntry : SoftDeletable
{
    /// <summary>
    /// Phiên bản của dòng dữ liệu.
    /// </summary>
    [JsonPropertyName("rowVersion")]
    [Timestamp]
    public uint RowVersion { get; set; }
}