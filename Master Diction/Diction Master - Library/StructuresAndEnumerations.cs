


using System;
using System.Collections.Generic;
using System.Security.Permissions;

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
        Add,
        Edit,
        Delete
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
        Question,
        Uknown
    }

    public enum QuestionType
    {
        Text,
        Puzzle,
        Choice
    }

    public enum ApplicationType
    {
        Diction,
        Teachers,
        Audio
    }

    public enum SubscriptionType
    {
        Term,
        Year
    }

    public enum Action
    {
        Login,
        Register
    }

    public enum KeyValidation
    {
        ValidOneTerm,
        ValidFullYear,
        Invalid
    }

    [Serializable]
    public struct ContentVersionInfo
    {
        public long ComponentID;
        public long ParentID;
        public ContentStatus Status;
        public Component Component;
    }

    [Serializable]
    public struct Subscription
    {
        public long ID { get; set; }
        public long ClientID { get; set; }
        public long CourseID { get; set; }
        public long EduLevelID { get; set; }
        public long GradeID { get; set; }
        public long TermID { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public string Key { get; set; }
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