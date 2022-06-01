using System.Net;
using LearnBotServer.API;
using LearnBotServer.Model;
using LearnBotVrk.Vkr.API;
using Microsoft.AspNetCore.Mvc;

namespace LearnBotServer.Controllers;

[Route("api/{userId}")]
public class UserController : Controller
{
    private UserContext db;

    public UserController(UserContext context)
    {
        db = context;
    }

    [HttpPost]
    [Route("register")]
    public IActionResult Register(
        long userId,
        [Bind(Prefix = "first_name")] string firstName,
        [Bind(Prefix = "last_name")] string lastName,
        [Bind(Prefix = "phone_number")] string phoneNumber
    )
    {
        User user = new User()
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Id = userId
        };

        db.Users.Add(user);
        db.SaveChanges();

        return VkrResponse.OK(true);
    }

    [HttpGet]
    [Route("")]
    public IActionResult IsRegistered(long userId)
    {
        var user = db.Users.Find(userId);
        if (user != null)
        {
            return VkrResponse.OK(true);
        }

        return VkrResponse.OK(false);
    }

    [HttpGet]
    [Route("courses")]
    public IActionResult GetCourses(long userId)
    {
        try
        {
            var user = db.Users.Find(userId);
            if (user != null)
            {
                var courses = db.UserCourses
                    .Where(c => c.User == user)
                    .Select(c => Resources.GetCourse(c.CourseId))
                    .ToList();

                return VkrResponse.OK(courses);
            }

            return VkrResponse.Failed(HttpStatusCode.Forbidden);
        }
        catch (KeyNotFoundException e)
        {
            return VkrResponse.Failed(HttpStatusCode.NotFound);
        }
        catch (FileNotFoundException e)
        {
            return VkrResponse.Failed(HttpStatusCode.NotFound);
        }
    }

    [HttpGet]
    [Route("profile")]
    public IActionResult GetProfile(long userId)
    {
        var user = db.Users.Find(userId);
        if (user != null)
        {
            var courses = db.UserCourses
                .Where(c => c.User == user)
                .Select(c => Resources.GetCourse(c.CourseId))
                .ToList();

            var quizzes = db.CompletedQuizzes
                .Where(q => q.UserId == userId);

            var items = new List<dynamic>();

            foreach (var course in courses)
            {
                int total = courses
                    .Select(c => c.GetCourseChapters())
                    .Sum(l => l.Sum(ch => ch.Paragraphs.Count));

                int quizesTotal = quizzes
                    .Count(q => q.CourseId == course.Id);

                int done = db.Progresses
                    .Count(p => p.CourseId == course.Id && p.UserId == userId);

                float progress = (float)(done + quizesTotal) / (total + course.GetCourseChapters().Count);

                items.Add(new
                {
                    course_id = course.Id,
                    title = course.Title,
                    progress = progress,
                    total_quiz_num = course.GetCourseChapters().Count,
                    completed_quiz_num = quizesTotal,
                    total_pages_num = total,
                    completed_pages_num = done
                });
            }

            return VkrResponse.OK(items);
        }

        return VkrResponse.Failed();
    }
}