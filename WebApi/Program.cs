using DataAccess;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<QuizRepository>();
builder.Services.AddTransient<CategoryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.AddUserRoutes();

app.AddQuizRoutes();

app.AddCategoryRoutes();

app.Run();
