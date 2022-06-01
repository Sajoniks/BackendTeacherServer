using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace LearnBotServer.API
{
    public class CourseChapter
    {
        [YamlIgnore] [JsonIgnore] public int Id { get; set; }

        [YamlIgnore] [JsonIgnore] public Course Course { get; set; }

        public String Title { get; set; }
        public Dictionary<String, CourseParagraph> Paragraphs { get; set; }
    }
}