using LearnBotServer.API;

namespace LearnBotServer.Model;

public class UserCourse
{
    public int Id { get; set; }
    public User User { get; set; }
    public String CourseId { get; set; }
}