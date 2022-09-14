using ApiUser.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApiUser.Services;

public class UsersService
{
    private readonly IMongoCollection<User> _usersCollection;

    public UsersService(
        IOptions<ApiUsersDataBaseSettings> userStoreDatabaseSettings)
    {
        MongoClient mongoClient = new MongoClient(
            userStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            userStoreDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<User>(
            userStoreDatabaseSettings.Value.CollectionName);
    }

    public async Task<List<User>> GetAsync() =>
        await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetAsync(string id) =>
        await _usersCollection.Find(x => x.userId == id).FirstOrDefaultAsync();
    public async Task<User?> GetUserAsync(string user) =>
        await _usersCollection.Find(x => x.nameUser == user).FirstOrDefaultAsync();
    public async Task<User?> GetCountAsync(string count) =>
        await _usersCollection.Find(x => x.countNumber == count).FirstOrDefaultAsync();
    public async Task<User?> FilterNameAsync(string name) =>
        await _usersCollection.Find(x => x.name == name).FirstOrDefaultAsync();
    public async Task CreateAsync(User newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

    public async Task UpdateAsync(string id, User updateUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.userId == id, updateUser);
    public async Task UpdateUserAsync(string user, User updateUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.nameUser == user, updateUser);
    public async Task UpdateCountAsync(string count, User updateUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.countNumber == count, updateUser);

    public async Task RemoveAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.userId == id);

}