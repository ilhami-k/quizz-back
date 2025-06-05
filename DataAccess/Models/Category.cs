using System;
using System.Security.Policy;

namespace Infrastructure.Models;

public class Category
{
    public int CategoryId {get;set;}
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt {get;set;}
}
