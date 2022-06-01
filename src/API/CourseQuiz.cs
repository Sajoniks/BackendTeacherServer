using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace LearnBotServer.API
{
    public class CourseQuiz
    {
        public class Question
        {
            [YamlMember(Alias = "text")]
            [JsonPropertyName("text")]
            public string Text { get; private set; }

            [JsonPropertyName("paragraph_id")]
            [YamlMember(Alias = "page")]
            public String ParagraphId { get; private set; }

            [JsonPropertyName("options")]
            [YamlMember(Alias = "opts")]
            public string[] OptionStrings { get; private set; }

            [JsonPropertyName("correct_option_num")]
            [YamlMember(Alias = "correct")]
            public int CorrectOptionId { get; private set; }
        }

        [YamlMember(Alias = "quiz")]
        [JsonPropertyName("questions")]
        public List<Question> Questions { get; private set; }
    }
}