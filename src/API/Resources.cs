using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using LearnBotServer.API;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LearnBotVrk.Vkr.API
{
    public static class Resources
    {
        private static readonly string ResoursesPath;
        private static readonly Dictionary<string, string> CoursesMapping = new Dictionary<string, string>();
        private static readonly IDeserializer Deserializer;

        private static class Constants
        {
            public static readonly string Chapters = "chapters";
            public static readonly string Paragraphs = "paragraph";
            public static readonly string Resources = "resources";

            public static readonly string InfoFileName = "info";
            public static readonly string InfoFileExtension = "yaml";

            public static readonly string QuizFileName = "quiz";
            public static readonly string QuizFileExtension = "yaml";

            public static string QuizFile => $"{QuizFileName}.{QuizFileExtension}";
            public static string InfoFile => $"{InfoFileName}.{InfoFileExtension}";

            public static readonly string ParagraphFileExtension = "txt";
            public static string ParagraphFile(string id) => $"{id}.{ParagraphFileExtension}";
        }

        private static String GetCoursePath(Course course) => Path.GetDirectoryName(CoursesMapping[course.Id])!;
        private static String GetChaptersPath(Course course) => Path.Combine(GetCoursePath(course), Constants.Chapters);

        private static String GetChapterPath(CourseChapter chapter) =>
            Path.Combine(GetChaptersPath(chapter.Course), chapter.Id.ToString());

        private static String GetParagraphsPath(CourseChapter chapter) =>
            Path.Combine(GetChapterPath(chapter), Constants.Paragraphs);

        private static String GetParagraphPath(CourseParagraph paragraph) =>
            GetParagraphPath(paragraph.Chapter, paragraph.Id);

        private static String GetParagraphPath(CourseChapter chapter, String id) =>
            Path.Combine(GetParagraphsPath(chapter), Constants.ParagraphFile(id));

        private static String GetQuizPath(CourseChapter chapter) =>
            Path.Combine(GetChapterPath(chapter), Constants.QuizFile);

        static Resources()
        {
            // root
            ResoursesPath = Path.Combine(Environment.CurrentDirectory, Constants.Resources);

            // yaml deserializer
            Deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            // build map course.id <=> course file path
            var dirs = Directory
                .EnumerateDirectories(ResoursesPath, "*", SearchOption.TopDirectoryOnly)
                .Select(s =>
                {
                    var path = Path.Combine(s, Constants.InfoFile);
                    var yml = File.ReadAllText(path);
                    var info = Deserializer.Deserialize<Course>(yml);

                    return new KeyValuePair<string, string>(info.Id, path);
                });

            foreach (var dir in dirs)
                CoursesMapping.Add(dir.Key, dir.Value);
        }

        public static CourseChapter GetCourseChapter(this Course course, int chapter)
        {
            var chapterYml = Path.Combine(GetChaptersPath(course), chapter.ToString(), Constants.InfoFile);

            var chap = Deserializer.Deserialize<CourseChapter>(File.ReadAllText(chapterYml));
            chap.Id = chapter;
            chap.Course = course;

            foreach (var courseParagraph in chap.Paragraphs)
            {
                courseParagraph.Value.Chapter = chap;
                courseParagraph.Value.Id = courseParagraph.Key;
            }

            return chap;
        }

        public static List<CourseChapter> GetCourseChapters(this Course course)
        {
            return
                Directory
                    .GetDirectories(GetChaptersPath(course), "*", SearchOption.TopDirectoryOnly)
                    .Select(dir => course.GetCourseChapter(int.Parse(Path.GetFileName(dir))))
                    .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Course GetCourse(string id) =>
            Deserializer.Deserialize<Course>(File.ReadAllText(CoursesMapping[id]));

        public static String GetParagraphText(this CourseParagraph paragraph) =>
            File.ReadAllText(GetParagraphPath(paragraph));

        public static string GetParagraphText(this CourseChapter chapter, string id) =>
            File.ReadAllText(GetParagraphPath(chapter, id));

        public static CourseQuiz GetChapterQuiz(this CourseChapter chapter)
        {
            var quizPath = GetQuizPath(chapter);
            var content = File.ReadAllText(quizPath);
            var quiz = Deserializer.Deserialize<CourseQuiz>(content);
            return quiz;
        }
    }
}