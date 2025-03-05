using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Safesfir.Data
{
    
    public class MongoDBService
    {
        private readonly IMongoCollection<User> _users;
        private static string _database_url= "mongodb://safesfir:Vhewdz2dbhs62WE@34.202.178.64:27017/test";
        private static string _database_nam = "test";
        private static string _database_table = "users";
        public MongoDBService()
        {
            var client = new MongoClient(_database_url);
            var database = client.GetDatabase(_database_nam);
            _users = database.GetCollection<User>(_database_table);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _users.Find(user => true).ToListAsync();
        }


        public async Task<List<User>> GetDriversAsync()
        {
            return await _users.Find(user =>  user.Role.ToLower() == "driver" && user.QuickBooks.Type== QuickBooksType.QBWD).ToListAsync();
        }
        public async Task<User> GetDriverByNameAsync(string name)
        {
            //
            return await _users.Find(user => user.Name.ToLower() == name.ToLower()&& user.Role.ToLower() == "driver"  ).FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByQBDUserNameAndPawordAsync(string username,string password)
        {
            return await _users.Find(user => user.QuickBooks!=null&& user.QuickBooks.QbwdPassword == password && user.QuickBooks.QbwdUsername == username).FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task UpdateUserAsync(string id, User user)
        {
            await _users.ReplaceOneAsync(u => u.Id == id, user);
        }

        public async Task DeleteUserAsync(string id)
        {
            await _users.DeleteOneAsync(u => u.Id == id);
        }
    }

}
