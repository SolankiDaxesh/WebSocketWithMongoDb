using MongoDB.Driver;
using WebSocket_Demo.Models;

namespace WebSocket_Demo.Services
{
    public interface IMongoDbService
    {
        Task SaveLocationUpdate(LocationUpdate locationUpdate);
    }
    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoCollection<LocationUpdate> _locationCollection;

        public MongoDbService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("locationDb");
            _locationCollection = database.GetCollection<LocationUpdate>("locations");
        }

        public async Task SaveLocationUpdate(LocationUpdate locationUpdate)
        {
            locationUpdate.Timestamp = DateTime.UtcNow;
            await _locationCollection.InsertOneAsync(locationUpdate);
        }
    }
}
