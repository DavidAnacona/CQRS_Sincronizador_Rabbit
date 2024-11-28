using MongoDB.Bson.Serialization.Attributes;

namespace ConsultaService.Entities
{
    public class RecetaEntity
    {

        [BsonElement("IdReceta")]
        public int IdReceta { get; set; }

        [BsonElement("IdConsulta")]
        public int IdConsulta { get; set; }

        [BsonElement("Medicamentos")]
        public string Medicamentos { get; set; } = null!;

        [BsonElement("Indicaciones")]
        public string? Indicaciones { get; set; }
    }
}
