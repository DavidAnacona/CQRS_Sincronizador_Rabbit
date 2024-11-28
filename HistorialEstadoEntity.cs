using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultaService.Entities
{
    public class HistorialEstadoEntity
    {

        [BsonElement("IdHistorialEstado")]
        public int IdHistorialEstado { get; set; }

        [BsonElement("IdConsulta")]
        public int IdConsulta { get; set; }

        [BsonElement("IdEstadoConsulta")]
        public int IdEstadoConsulta { get; set; }

        [BsonElement("FechaCambio")]
        public DateTime FechaCambio { get; set; }

        [BsonElement("UsuarioResponsable")]
        public string? UsuarioResponsable { get; set; }

        [BsonElement("Comentario")]
        public string? Comentario { get; set; }
    }
}
