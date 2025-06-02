using Api.EndPoints;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Abstractions; 
using Api.Middleware;
using Scalar;
using Scalar.AspNetCore;
using Core.IGateway;
using Infrastructure.Gateways;
using Core.UseCases.Abstractions;
using Core.UseCases;

var builder = WebApplication.CreateBuilder(args);

// Define a CORS policy name
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.

builder.Services.AddOpenApi();
builder.Services.AddTransient<IUserRepository, UserRepository>(); //
builder.Services.AddTransient<IQuizRepository, QuizRepository>(); 
builder.Services.AddTransient<CategoryRepository>();
builder.Services.AddTransient<IQuestionRepository, QuestionRepository>(); 
builder.Services.AddTransient<IUserGateway, UserGateway>();
builder.Services.AddTransient<IUserUseCases, UserUseCases>();


// Add CORS services 
// ATTENTION Il faut changer car la tout le monde peut acceder a l'API
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(
                                             "http://localhost:4200", // Localhost pour Angular
                                             "https://localhost:7223"// Scalar
                                            )
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{

    app.MapOpenApi(); //
    app.MapScalarApiReference();

}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

app.AddUserRoutes();
app.AddQuizRoutes();
app.AddCategoryRoutes();
app.AddQuestionRoutes(); 

app.Run();