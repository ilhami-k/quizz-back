using Api.EndPoints;
using Infrastructure.repositories;
using Infrastructure.Repositories.Abstractions; 
using Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Define a CORS policy name
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddTransient<UserRepository>(); 
builder.Services.AddTransient<IQuizRepository, QuizRepository>(); 
builder.Services.AddTransient<CategoryRepository>();
builder.Services.AddTransient<IQuestionRepository, QuestionRepository>(); 

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

app.AddUserRoutes();
app.AddQuizRoutes();
app.AddCategoryRoutes();
app.AddQuestionRoutes(); 

app.Run();