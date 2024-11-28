using ConsultaService.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CQRS_SyncService
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Configurar mapeo para ignorar el campo _id en todas las entidades
            BsonClassMap.RegisterClassMap<ConsultaEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<EstadoConsultaEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<HistorialEstadoEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<MedicoEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<PacienteEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<RecetaEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            // Configurar MongoDB
            var mongoClient = new MongoClient("mongodb+srv://admin:admin.mongodb@cqrs-distribuidos.stzvi.mongodb.net/?retryWrites=true&w=majority&appName=cqrs-distribuidos");
            var database = mongoClient.GetDatabase("GestionSalud");

            // Colecciones
            var consultaCollection = database.GetCollection<ConsultaEntity>("Consultas");
            var estadoConsultaCollection = database.GetCollection<EstadoConsultaEntity>("EstadoConsulta");
            var historialEstadoCollection = database.GetCollection<HistorialEstadoEntity>("HistorialEstadoConsulta");
            var medicoCollection = database.GetCollection<MedicoEntity>("Medicos");
            var pacienteCollection = database.GetCollection<PacienteEntity>("Pacientes");
            var recetaCollection = database.GetCollection<RecetaEntity>("Recetas");

            // Configurar RabbitMQ
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declarar colas
            DeclareQueue(channel, "ConsultaQueue");
            DeclareQueue(channel, "EstadoConsultaQueue");
            DeclareQueue(channel, "HistorialEstadoQueue");
            DeclareQueue(channel, "MedicoQueue");
            DeclareQueue(channel, "PacienteQueue");
            DeclareQueue(channel, "RecetaQueue");

            // Consumidores
            StartConsumer(channel, "ConsultaQueue", consultaCollection);
            StartConsumer(channel, "EstadoConsultaQueue", estadoConsultaCollection);
            StartConsumer(channel, "HistorialEstadoQueue", historialEstadoCollection);
            StartConsumer(channel, "MedicoQueue", medicoCollection);
            StartConsumer(channel, "PacienteQueue", pacienteCollection);
            StartConsumer(channel, "RecetaQueue", recetaCollection);

            Console.WriteLine("Esperando mensajes para todas las entidades...");
            Console.ReadLine();
        }

        private static void DeclareQueue(IModel channel, string queueName)
        {
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        private static void StartConsumer<T>(IModel channel, string queueName, IMongoCollection<T> collection) where T : class
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    // Deserializar el mensaje como un objeto dinámico
                    var deleteMessage = JsonSerializer.Deserialize<JsonElement>(message);

                    if (deleteMessage.TryGetProperty("Action", out var actionElement) && actionElement.GetString() == "Deleted")
                    {
                        // Obtener el campo de ID dinámicamente
                        var idPropertyName = "Id" + typeof(T).Name.Replace("Entity", "");
                        if (deleteMessage.TryGetProperty(idPropertyName, out var idElement) && idElement.ValueKind == JsonValueKind.Number)
                        {
                            var idValue = idElement.GetInt32();
                            var filter = Builders<T>.Filter.Eq(idPropertyName, idValue);
                            var deleteResult = await collection.DeleteOneAsync(filter);

                            if (deleteResult.DeletedCount > 0)
                            {
                                Console.WriteLine($"Entidad eliminada: {typeof(T).Name} con ID: {idValue}");
                            }
                            else
                            {
                                Console.WriteLine($"No se encontró la entidad para eliminar: {typeof(T).Name} con ID: {idValue}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"El mensaje no contiene un ID válido para {typeof(T).Name}");
                        }
                    }
                    else
                    {
                        // Procesar como inserción o actualización
                        var entity = JsonSerializer.Deserialize<T>(message);
                        if (entity != null)
                        {
                            var idProperty = typeof(T).GetProperty("Id" + typeof(T).Name.Replace("Entity", ""));
                            if (idProperty == null)
                            {
                                Console.WriteLine($"La entidad {typeof(T).Name} no contiene una propiedad de ID válida.");
                                return;
                            }

                            var idValue = idProperty.GetValue(entity);
                            var filter = Builders<T>.Filter.Eq("Id" + typeof(T).Name.Replace("Entity", ""), idValue);

                            var existingEntity = await collection.Find(filter).FirstOrDefaultAsync();

                            if (existingEntity == null)
                            {
                                await collection.InsertOneAsync(entity);
                                Console.WriteLine($"Nueva entidad insertada: {typeof(T).Name} con ID: {idValue}");
                            }
                            else
                            {
                                await collection.ReplaceOneAsync(filter, entity);
                                Console.WriteLine($"Entidad actualizada: {typeof(T).Name} con ID: {idValue}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando mensaje de {typeof(T).Name}: {ex.Message}");
                }
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }   
    }
}
