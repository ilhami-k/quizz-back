using System;
using System.Runtime.CompilerServices;

namespace DataAccess;

public class QuestionRepository
{
    private readonly string _connectionString;

    public QuestionRepository()
    {
        _connectionString = "Server=localhost;Database=quizzv2;User=root;Password=admin;";
    }

}
