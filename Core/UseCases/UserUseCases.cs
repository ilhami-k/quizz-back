using Core.Models;
using Core.UseCases.Abstractions;
using Core.IGateway;

namespace Core.UseCases;

public class UserUseCases : IUserUseCases
{
    private readonly IUserGateway _userGateway;

    public UserUseCases(IUserGateway userGateway)
    {
        _userGateway = userGateway ?? throw new ArgumentNullException(nameof(userGateway));
    }

    public User AuthentificateAndGetUser(AuthentificationRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Username and password are required.", nameof(request));
        }

        var user = _userGateway.GetUserByUsername(request.Username);
        if (user == null)
        {
            throw new ArgumentException("Invalid username or password.");
        }

        var hashedPassword = _userGateway.GetUserPasswordHash(request.Username);
        if (string.IsNullOrEmpty(hashedPassword))
        {
            throw new InvalidOperationException("Could not retrieve password for user.");
        }

        if (BCrypt.Net.BCrypt.Verify(request.Password, hashedPassword))
        {
            return user;
        }

        throw new ArgumentException("Invalid username or password.");
    }
    public IEnumerable<User> GetAllUsers()
    {
        var users = _userGateway.GetAllUsers();
        return users;
    }

    public void Register(RegisterRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
        {
            throw new ArgumentException("Invalid registration request");
        }

        if (request.Password != request.ConfirmPassword)
        {
            throw new ArgumentException("Passwords do not match");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var newUser = new User
        {
            Username = request.Username,
            Email = request.Email,
            IsAdmin = false
        };
        _userGateway.AddUser(newUser, hashedPassword);
    }
}