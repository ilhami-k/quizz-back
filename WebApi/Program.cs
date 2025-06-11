using Api.EndPoints;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Abstractions;
using Api.Middleware;
using Scalar.AspNetCore;
using Core.IGateway;
using Infrastructure.Gateways;
using Core.UseCases.Abstractions;
using Core.UseCases;
using Core.IGateways;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddOpenApi();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserGateway, UserGateway>();
builder.Services.AddTransient<IUserUseCases, UserUseCases>();

builder.Services.AddTransient<IQuizRepository, QuizRepository>();
builder.Services.AddTransient<IQuestionRepository, QuestionRepository>();
builder.Services.AddTransient<IAnswerRepository, AnswerRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>(); 

builder.Services.AddTransient<IQuizGateway, QuizGateway>();
builder.Services.AddTransient<IQuizUseCases, QuizUseCases>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(
                                             "http://localhost:4200", 
                                             "https://localhost:7223",
                                             "http://4.180.236.182"
                                            )
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

app.AddUserRoutes();
app.AddQuizRoutes();
app.AddCategoryRoutes();
app.AddQuestionRoutes();
//

app.Run();
