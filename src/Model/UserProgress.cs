namespace LearnBotServer.Model;

public class UserProgress
{
    public int Id { get; set; }
    public long UserId { get; set; }

    public string CourseId { get; set; }
    public int ChapterId { get; set; }
    public string ParagraphId { get; set; }
}