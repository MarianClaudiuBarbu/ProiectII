using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GoMedApp.Models;

public class PatientModel
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Name { get; set; } = null!;
    public int Age { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

}
