using System;
using Core.IGateway;
using Infrastructure.Repositories.Abstractions;
using Infrastructure.Models;

namespace Infrastructure.Gateways;

public class UserGateway : IUserGateway
{
    private readonly IUserRepository _userRepository;

    public UserGateway(IUserRepository userRepository)
    {
        _userRepository = userRepository
            ?? throw new ArgumentNullException(nameof(userRepository));
    }
    public void AddUser(Core.Models.User user, string passwordHash)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentNullException(nameof(passwordHash), "Password hash cannot be null or empty.");
        }

        var newInfraUser = new User
        {
            Username = user.Username,
            Email = user.Email,
            IsAdmin = user.IsAdmin,
            PasswordHash = passwordHash
        };

        _userRepository.AddUser(newInfraUser);
    }
    public void DeleteUser(int userId)
    {
        if (userId >= 0)
        {
            _userRepository.DeleteUser(userId);
        }
        
    }

    public string? GetUserPasswordHash(string username)
    {
        var user = _userRepository.GetUserByUsername(username);
        return user?.PasswordHash;
    }
    public IEnumerable<Core.Models.User> GetAllUsers()
    {
        var users = _userRepository.GetAllUsers();
        return users.Select(user => new Core.Models.User
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            PhotoURL = user.PhotoURL,
            IsAdmin = user.IsAdmin,
            CreatedQuizzes = user.CreatedQuizzes,
            ParticipatedQuizzes = user.ParticipatedQuizzes
        });
    }
    public Core.Models.User? GetUserById(int userId)
    {
        var user = _userRepository.GetUserById(userId);
        if (user == null) return null;
        return new Core.Models.User
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
        if (user == null) return null;
        return new Core.Models.User
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedQuizzes = user.CreatedQuizzes,
            ParticipatedQuizzes = user.ParticipatedQuizzes
        };
    }

    public void UpdateUser(Core.Models.User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));

        }

        var infraUser = new Infrastructure.Models.User
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email
        };

        _userRepository.UpdateUser(infraUser); 
    }

}