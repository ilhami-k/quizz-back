using System;
using System.Security.Policy;

namespace Infrastructure.Models;

public class Category
{
    public int CategoryId {get;set;}
    public required string Name {get;set;}
    public DateTime CreatedAt {get;set;}
}
