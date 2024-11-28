using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultaService.Entities
{
    public class PacienteEntity
    {

        [BsonElement("IdPaciente")]
        public int IdPaciente { get; set; }

        [BsonElement("Nombre")]
        public string Nombre { get; set; } = null!;

        [BsonElement("Apellido")]
        public string Apellido { get; set; } = null!;

        [BsonElement("FechaNacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [BsonElement("Telefono")]
        public string? Telefono { get; set; }

        [BsonElement("Correo")]
        public string? Correo { get; set; }

        [BsonElement("Direccion")]
        public string? Direccion { get; set; }
    }
}
