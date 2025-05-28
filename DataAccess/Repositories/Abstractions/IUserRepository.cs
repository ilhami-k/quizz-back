using Infrastructure.Models;

namespace Infrastructure.Repositories.Abstractions;

public interface IUserRepository
{
    User? GetUserById(int userId);
    User? GetUserByUsername(string username);
    void AddUser(User user);
    IEnumerable<User> GetAllUsers();
}
