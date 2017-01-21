


using System.Collections.Generic;

namespace Diction_Master___Library
{
    public enum GradeType
    {
        NurseryI,
        NurseryII,
        PrimaryI,
        PrimaryII,
        PrimaryIII,
        PrimaryIV,
        PrimaryV,
        PrimaryVI,
        SecondaryJuniorI,
        SecondaryJuniorII,
        SecondaryJuniorIII,
        SecondarySeniorI,
        SecondarySeniorII,
        SecondarySeniorIII
    }

    public enum EducationalLevelType
    {
        Nursery,
        Primary,
        Secondary,
        Teachers
    }

    public enum ContentStatus
    {
        OutDated,
        UpToDate
    }

    public enum NetworkAvailability
    {
        Offline,
        Online
    }

    public enum ComponentType
    {
        Course,
        EducationalLevel,
        Grade,
        Week,
        Lesson,
        Audio,
        Video,
        Document,
        Quiz,
        Question
    }

    public enum QuestionType
    {
        Text,
        Puzzle
    }

    public struct ContentVersionInfo
    {
        public int ComponentID;
        public ContentStatus Status;
    }

    public static class Icons
    {
        public const string Nursery = "./Resources/nursery.png";
        public const string Primary = "./Resources/primary.png";
        public const string Secondary = "./Resources/secondary.png";
        public const string NurseryI = "";
        public const string NurseryII = "";
        public const string PrimaryI = "";
        public const string PrimaryII = "";
        public const string PrimaryIII = "";
        public const string PrimaryIV = "";
        public const string PrimaryV = "";
        public const string PrimaryVI = "";
        public const string SecondaryJuniorI = "";
        public const string SecondaryJuniorII = "";
        public const string SecondaryJuniorIII = "";
        public const string SecondarySeniorI = "";
        public const string SecondarySeniorII = "";
        public const string SecondarySeniorIII = "";
    }
}