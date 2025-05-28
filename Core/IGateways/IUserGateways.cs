using System;
using Core.Models;


namespace Core.IGateway;

public interface IUserGateway
{
    User? GetUserById(int userId);
    User? GetUserByName(string username);
    void AddUser(User user);
    IEnumerable<User> GetAllUsers();
    void DeleteUser(int userId);

    string? GetUserPasswordHash(int userId);
}
