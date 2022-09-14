using ApiUser.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApiUser.Services;

public class LoginService
{
    private readonly IMongoCollection<UserLogin> _loginCollection;

    public LoginService(
        IOptions<ApiUsersDataBaseSettings> userStoreDatabaseSettings)
    {
        MongoClient mongoClient = new MongoClient(
            userStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            userStoreDatabaseSettings.Value.DatabaseName);

        _loginCollection = mongoDatabase.GetCollection<UserLogin>("Login");
    }

    public async Task<List<UserLogin>> GetAsync() =>
        await _loginCollection.Find(_ => true).ToListAsync();

    public async Task<UserLogin?> GetAsync(string id) =>
        await _loginCollection.Find(x => x.id == id).FirstOrDefaultAsync();
    public async Task<UserLogin?> FilterNameAsync(string name) =>
        await _loginCollection.Find(x => x.user == name).FirstOrDefaultAsync();
    public async Task CreateAsync(UserLogin newUser) =>
        await _loginCollection.InsertOneAsync(newUser);

    public async Task UpdateAsync(string id, UserLogin updateUser) =>
        await _loginCollection.ReplaceOneAsync(x => x.id == id, updateUser);

    public async Task RemoveAsync(string id) =>
        await _loginCollection.DeleteOneAsync(x => x.id == id);

}