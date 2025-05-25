using Infrastructure.Models;

namespace Infrastructure.Repositories.Abstractions;

public interface IUserRepository
{
    User? GetUserById(int userId);
    void AddUser(User user);
    IEnumerable<User> GetAllUsers();
}
