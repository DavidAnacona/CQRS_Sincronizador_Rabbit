using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultaService.Entities
{
    public class MedicoEntity
    {

        [BsonElement("IdMedico")]
        public int IdMedico { get; set; }

        [BsonElement("Nombre")]
        public string Nombre { get; set; } = null!;

        [BsonElement("Apellido")]
        public string Apellido { get; set; } = null!;

        [BsonElement("Especialidad")]
        public string? Especialidad { get; set; }

        [BsonElement("Telefono")]
        public string? Telefono { get; set; }

        [BsonElement("Correo")]
        public string? Correo { get; set; }
    }
}
