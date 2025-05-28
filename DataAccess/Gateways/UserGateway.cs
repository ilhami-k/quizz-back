using System;
using Core.IGateway;
using Infrastructure.Repositories.Abstractions;
using Infrastructure.Models;

namespace Infrastructure.Gateways;

public class UserGateway
{
    private readonly IUserRepository _userRepository;

    public UserGateway(IUserRepository userRepository)
    {
        _userRepository = userRepository
            ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public void AddUser(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        _userRepository.AddUser(user);
    }
    public void DeleteUser(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero.");
        }
        _userRepository.DeleteUser(userId);
    }
    public IEnumerable<Core.Models.User> GetAllUsers()
    {
        var users = _userRepository.GetAllUsers();
        return users.Select(user => new Core.Models.User
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedQuizzes = user.CreatedQuizzes,
            ParticipatedQuizzes = user.ParticipatedQuizzes
        });
    }
    public Core.Models.User? GetUserById(int userId)
    {
        var user = _userRepository.GetUserById(userId);
        return user == null ? null : new Core.Models.User
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedQuizzes = user.CreatedQuizzes,
            ParticipatedQuizzes = user.ParticipatedQuizzes
        };
    }
    public Core.Models.User? GetUserByUsername(string username)
    {
        var user = _userRepository.GetUserByUsername(username);
        return user == null ? null : new Core.Models.User
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedQuizzes = user.CreatedQuizzes,
            ParticipatedQuizzes = user.ParticipatedQuizzes
        };
    }
}
