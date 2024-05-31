using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    public class AppointmentsModel
    {
            
            [BsonId]
            public ObjectId Id { get; set; }

            public string Name { get; set; } = null!;
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] 
            public DateTime Data { get; set; }
            [DataType(DataType.Time)]
            [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
            public TimeSpan Hour { get; set; }

    }
}
