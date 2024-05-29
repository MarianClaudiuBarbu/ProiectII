using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    public class PatientModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; } = null!;
        public int Age { get; set; }

        public string Email { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

    }
}
