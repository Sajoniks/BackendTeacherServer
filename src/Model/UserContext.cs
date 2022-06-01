using Microsoft.EntityFrameworkCore;

namespace LearnBotServer.Model;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserCourse> UserCourses { get; set; }
    public DbSet<UserProgress> Progresses { get; set; }
    public DbSet<UserQuiz> CompletedQuizzes { get; set; }
}