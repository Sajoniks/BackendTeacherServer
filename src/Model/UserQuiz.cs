namespace LearnBotServer.Model;

public class UserQuiz
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public string CourseId { get; set; }
    public int ChapterId { get; set; }
}