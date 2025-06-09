using System;
using Core.Models;


namespace Core.IGateway;

public interface IUserGateway
{
    User? GetUserById(int userId);
    User? GetUserByUsername(string username);
    void AddUser(User user, string passwordHash);
    IEnumerable<User> GetAllUsers();
    void DeleteUser(int userId);
    string? GetUserPasswordHash(string username);
    void UpdateUser(User user);
}
