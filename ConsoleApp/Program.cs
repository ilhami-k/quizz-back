using DataAccess;
using DataAccess.Models;

var userRepo = new UserRepository();
var categoryRepo = new CategoryRepository();
var quizRepo = new QuizRepository();


//categoryRepo.AddCategory(new Category{Name = "Autres", CreatedAt = DateTime.Now});
//userRepo.AddUser(new User {Name = "Clém", Email = "Clém@test.be", Password = "testmdp", IsAdmin = 1, CreatedAt = DateTime.Now, CreatedQuizzes = 0, ParticipatedQuizzes = 0});
//userRepo.DeleteUser(11);

var users = userRepo.GetAllUsers();
foreach (var user in users)
{
    var isAdmin = user.IsAdmin == 1 ? "Oui":"Non";
    Console.WriteLine($"{user.UserId} : {user.Name} (admin : {isAdmin})");
}

var categorys = categoryRepo.GetAllCategories();
foreach (var category in categorys)
{
    Console.WriteLine($"{category.CategoryId} : {category.Name}");
}

var quizzes = quizRepo.GetAllQuizzes();
foreach (var quizz in quizzes)
{
    Console.WriteLine($"({quizz.QuizId}) Titre : {quizz.Title}, description : {quizz.Description}, difficulté : {quizz.Dificulty}");
}

Console.WriteLine(categoryRepo.GetCategoryById(1));
var userUn = userRepo.GetUserById(1);
Console.Write($"Pseudo : {userUn?.Name} // Email : {userUn?.Email}");