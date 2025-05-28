using Core.Models;

namespace Core.UseCases.Abstractions;

public interface IUserUseCases
{
    User AuthentificateAndGetUser(AuthentificationRequest request);
    void Register(RegisterRequest request);
    IEnumerable<User> GetAllUsers();
}