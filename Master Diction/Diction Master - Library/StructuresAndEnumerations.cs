


namespace Diction_Master___Library
{
    public enum EducationalLevelType
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
        SecondarySeniorIII,
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