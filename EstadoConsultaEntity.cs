using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultaService.Entities
{
    public class EstadoConsultaEntity
    {

        [BsonElement("IdEstadoConsulta")]
        public int IdEstadoConsulta { get; set; }

        [BsonElement("NombreEstado")]
        public string NombreEstado { get; set; } = null!;
    }
}
