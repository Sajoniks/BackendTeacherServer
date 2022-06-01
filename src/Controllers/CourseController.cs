using System.Net;
using LearnBotServer.API;
using LearnBotServer.Model;
using LearnBotVrk.Vkr.API;
using Microsoft.AspNetCore.Mvc;

namespace LearnBotServer.Controllers;

[Route("api/{userId}/courses")]
public class CourseController : Controller
{
    private readonly UserContext db;

    public CourseController(UserContext ctx)
    {
        db = ctx;
    }


    [HttpPost]
    [Route("give")]
    public IActionResult GiveCourse(long userId, string courseId)
    {
        var user = db.Users.Find(userId);
        if (user != null)
        {
            var userCourse = new UserCourse()
            {
                CourseId = courseId,
                User = user
            };
            db.UserCourses.Add(userCourse);
            db.SaveChanges();
        }

        return Ok();
    }

    [HttpGet]
    [Route("{courseId}")]
    public IActionResult GetCourse(long userId, string courseId)
    {
        try
        {
            var user = db.Users.Find(userId);
            if (user != null)
            {
                var progressQuery = db.Progresses
                    .Where(p => p.UserId == user.Id && p.CourseId == courseId);

                var quizzesQuery = db.CompletedQuizzes
                    .Where(p => p.UserId == user.Id && p.CourseId == courseId);

                var courseData = Resources.GetCourse(courseId);
                var chaptersData = courseData.GetCourseChapters();

                var chaptersResponse =
                    chaptersData
                        .Select(c => new
                        {
                            Id = c.Id,
                            CourseId = courseData.Id,
                            Title = c.Title,
                            Completed = quizzesQuery.Any(q => q.ChapterId == c.Id && q.CourseId == courseId),
                            Paragraphs = c.Paragraphs
                                .Select(p => new
                                {
                                    Id = p.Key,
                                    CourseId = courseData.Id,
                                    ChapterId = c.Id,
                                    Title = p.Value.Title,
                                    Completed = progressQuery.Any(prg =>
                                        prg.ParagraphId == p.Key && prg.ChapterId == c.Id)
                                })
                        });

                var courseResponse = new
                {
                    Title = courseData.Title,
                    Id = courseData.Id,
                    Chapters = chaptersResponse
                };

                return VkrResponse.OK(courseResponse);
            }
            else
            {
                return VkrResponse.Failed(HttpStatusCode.Forbidden);
            }
        }
        catch (KeyNotFoundException exception)
        {
            return VkrResponse.Failed(HttpStatusCode.NotFound);
        }
        catch (FileNotFoundException e)
        {
            return VkrResponse.Failed(HttpStatusCode.NotFound);
        }
    }

    [HttpGet]
    [Route("{courseId}/chapter/{chapterId}/page/{paragraph}")]
    public IActionResult GetParagraphText(long userId, string courseId, int chapterId, string paragraph)
    {
        var user = db.Users.Find(userId);
        if (user != null)
        {
            var course = Resources.GetCourse(courseId);
            var chapter = course.GetCourseChapter(chapterId);
            var par = chapter.GetParagraphText(paragraph);

            return VkrResponse.OK(par);
        }

        return VkrResponse.Failed();
    }

    [HttpPost]
    [Route("{courseId}/chapter/{chapterId}/completeQuiz")]
    public IActionResult AddQuiz(long userId, string courseId, int chapterId)
    {
        if (db.CompletedQuizzes.FirstOrDefault(q => q.UserId == userId) == null)
        {
            var progression = new UserQuiz()
            {
                ChapterId = chapterId,
                CourseId = courseId,
                UserId = userId
            };

            db.CompletedQuizzes.Add(progression);
            db.SaveChanges();
        }

        return VkrResponse.OK(true);
    }

    [HttpPost]
    [Route("{courseId}/chapter/{chapterId}/complete")]
    public IActionResult AddProgression(long userId, string courseId, int chapterId, string page)
    {
        var user = db.Users.Find(userId);
        if (user != null)
        {
            var progression = new UserProgress()
            {
                ChapterId = chapterId,
                CourseId = courseId,
                ParagraphId = page,
                UserId = userId
            };

            db.Progresses.Add(progression);
            db.SaveChanges();
        }

        return VkrResponse.OK(true);
    }

    [HttpGet]
    [Route("{courseId}/chapter/{chapterId}/quiz")]
    public IActionResult GetQuizForChapter(long userId, string courseId, int chapterId)
    {
        var course = Resources.GetCourse(courseId);
        var chapter = course.GetCourseChapter(chapterId);
        var quiz = chapter.GetChapterQuiz();

        return VkrResponse.OK(new
        {
            course_id = course.Id,
            chapter_id = chapter.Id,
            questions = quiz.Questions.Select(q => new
            {
                text = q.Text,
                paragraph_id = q.ParagraphId,
                title = chapter.Paragraphs[q.ParagraphId].Title,
                correct_option_num = q.CorrectOptionId,
                options = q.OptionStrings,
            })
        });
    }
}