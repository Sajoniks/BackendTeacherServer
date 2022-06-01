using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace LearnBotServer.API
{
    public class CourseParagraph
    {
        [YamlIgnore] [JsonIgnore] public String Id { get; set; }

        [YamlIgnore] [JsonIgnore] public CourseChapter Chapter { get; set; }

        public String Title { get; set; }
    }
}