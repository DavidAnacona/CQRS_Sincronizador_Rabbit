using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultaService.Entities
{
    public class ConsultaEntity
    {

        [BsonElement("IdConsulta")]
        public int IdConsulta { get; set; }

        [BsonElement("IdPaciente")]
        public int IdPaciente { get; set; }

        [BsonElement("FechaHora")]
        public DateTime FechaHora { get; set; }

        [BsonElement("IdEstadoConsulta")]
        public int IdEstadoConsulta { get; set; }

        [BsonElement("IdMedico")]
        public int IdMedico { get; set; }

        [BsonElement("Notas")]
        public string? Notas { get; set; }
    }
}