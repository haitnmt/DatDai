using MongoDB.Bson;

namespace Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;

public interface IBaseEntry
{
    ObjectId Id { get; set; }
    string? Metadata { get; set; }
    DateTime StartTimeUtc { get; set; }
    DateTime? EndTimeUtc { get; set; }
    bool Success { get; set; }
    string? Exception { get; set; }
}