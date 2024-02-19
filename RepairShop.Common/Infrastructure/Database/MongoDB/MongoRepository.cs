using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RepairShop.Infrastructure;
using System.Linq.Expressions;

namespace RepairShop.Common.Infrastructure.Database.MongoDB
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;
        private readonly ILogger<MongoRepository<T>> _logger;
        public MongoRepository(IMongoDatabase mongoDatabase, string collectionName, ILogger<MongoRepository<T>> logger)
        {
            dbCollection = mongoDatabase.GetCollection<T>(collectionName);
            _logger = logger;
        }
        public async Task<IReadOnlyCollection<T>> GetAllListAsync()
        {
            try
            {
                return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
            }
            catch (MongoCommandException ex)
            {
                _logger.LogError(ex, "[{0}-{1}] A database error occurred while executing {2}", nameof(MongoRepository<T>), typeof(T).Name, nameof(GetAllListAsync));
                throw;
            }
        }
        public async Task<IReadOnlyCollection<T>> GetAllListAsync(Expression<Func<T, bool>> filter)
        {
            try
            {
                return await dbCollection.Find(filter).ToListAsync();
            }
            catch (MongoCommandException ex)
            {
                _logger.LogError(ex, "[{0}-{1}] A database error occurred while executing {2}", nameof(MongoRepository<T>), typeof(T).Name, nameof(GetAllListAsync));
                throw;
            }
        }
        public async Task<T> GetAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("[{0}-{1}] Executing {2} for Id = {3}", nameof(MongoRepository<T>), typeof(T).Name, nameof(GetAsync), id);
                FilterDefinition<T> filterDefinition = filterBuilder.Eq(entity => entity.Id, id);
                return await dbCollection.Find(filterDefinition).FirstOrDefaultAsync();
            }
            catch (MongoCommandException ex)
            {
                _logger.LogError(ex, "[{0}-{1}] A database error occurred while executing {2} for Id = {3}", nameof(MongoRepository<T>), typeof(T).Name, nameof(GetAsync), id);
                throw;
            }
        }
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            try
            {
                return await dbCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (MongoCommandException ex)
            {
                _logger.LogError(ex, "[{0}-{1}] A database error occurred while executing {2}", nameof(MongoRepository<T>), typeof(T).Name, nameof(GetAsync));
                throw;
            }
        }
        public async Task CreateAsync(T entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            try
            {
                _logger.LogInformation("[{0}-{1}] Executing {2} for Id = {3}", nameof(MongoRepository<T>), typeof(T).Name, nameof(CreateAsync), entity.Id);
                await dbCollection.InsertOneAsync(entity);
            }
            catch (MongoCommandException ex)
            {
                _logger.LogError(ex, "[{0}-{1}] A database error occurred while executing {2} for Id = {3}", nameof(MongoRepository<T>), typeof(T).Name, nameof(CreateAsync), entity.Id);
                throw;
            }
        }
        public async Task UpdateAsync(T entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            try
            {
                _logger.LogInformation("[{0}-{1}] Executing {2} for Id = {3}", nameof(MongoRepository<T>), typeof(T).Name, nameof(UpdateAsync), entity.Id);
                FilterDefinition<T> filterDefinition = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
                await dbCollection.ReplaceOneAsync(filterDefinition, entity);
            }
            catch (MongoCommandException ex)
            {
                _logger.LogError(ex, "[{0}-{1}] A database error occurred while executing {2} for Id = {3}", nameof(MongoRepository<T>), typeof(T).Name, nameof(UpdateAsync), entity.Id);
                throw;
            }
        }
        public async Task RemoveAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("[{0}-{1}] Executing {2} for Id = {3}", nameof(MongoRepository<T>), typeof(T).Name, nameof(RemoveAsync), id);
                FilterDefinition<T> filterDefinition = filterBuilder.Eq(entity => entity.Id, id);
                await dbCollection.DeleteOneAsync(filterDefinition);
            }
            catch (MongoCommandException ex)
            {
                _logger.LogError(ex, "[{0}-{1}] A database error occurred while executing {2} for Id = {3}", nameof(MongoRepository<T>), typeof(T).Name, nameof(RemoveAsync), id);
                throw;
            }
        }
    }
}
