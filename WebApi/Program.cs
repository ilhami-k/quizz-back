using Api.EndPoints;
using Infrastructure.repositories;
using Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Define a CORS policy name
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
builder.Services.AddOpenApi(); //
builder.Services.AddTransient<UserRepository>(); //
builder.Services.AddTransient<QuizRepository>(); //
builder.Services.AddTransient<CategoryRepository>(); //

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.AllowAnyOrigin() // Replace with your Angular app's origin if different
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionHandlerMiddleware>(); // Custom global exception handler middleware

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); //
}

app.UseHttpsRedirection(); //

// Use the CORS policy
// This should be placed after UseRouting (which is implicitly added in minimal APIs)
// and before UseAuthorization (if you have it) and before mapping your controllers/routes.
app.UseCors(MyAllowSpecificOrigins);

app.AddUserRoutes(); //
app.AddQuizRoutes(); //
app.AddCategoryRoutes(); //

app.Run(); //