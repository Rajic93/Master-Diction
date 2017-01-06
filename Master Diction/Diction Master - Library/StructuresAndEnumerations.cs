


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
        Video
    }

    public struct ContentVersionInfo
    {
        public int ComponentID;
        public ContentStatus Status;
    }


}