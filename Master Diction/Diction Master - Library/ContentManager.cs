using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Diction_Master___Library
{
    public class ContentManager : ISubject, IMigrate
    {
        private static List<Component> courses { get; set; }
        private Component buildingCourse;
        private Dictionary<int, Component> componentsCache;
        private static int previousComponentID;
        private static int currentComponentID;

        private ClientManager clientManager;
        private static ContentManager contentManager;
        private static object _lock = new object();

        private ContentManager()
        {
            courses = new List<Component>();
            componentsCache = new Dictionary<int, Component>();
            clientManager = ClientManager.CreateInstance();
            LoadManifest();
        }

        private void LoadManifest()
        {
            
        }

        public static ContentManager CreateInstance()
        {
            lock (_lock)
            {
                return contentManager ?? new ContentManager();
            }
        }

        public void Start()
        {
            new Thread(() =>
            {
                while (true)
                {
                    
                }
            }).Start();
        }

        public List<ContentVersionInfo> CheckStatus(List<ContentVersionInfo> contentVersions)
        {
            throw new NotImplementedException();
        }

        public Component CreateComponent(ComponentType type)
        {
            switch (type)
            {
                case ComponentType.Course:
                    return new Course();
                case ComponentType.EducationalLevel:
                    return new EducationalLevel();
                case ComponentType.Grade:
                    return new Grade();
                case ComponentType.Week:
                    return new Week();
                case ComponentType.Lesson:
                    return new Lesson();
                case ComponentType.Audio:
                    return new LeafComponent() {ComponentType = ComponentType.Audio};
                case ComponentType.Video:
                    return new LeafComponent() {ComponentType = ComponentType.Video};
                case ComponentType.Document:
                    return new LeafComponent() {ComponentType = ComponentType.Document};
                default:
                    return null;
            }
        }
    
        public Component GetComponent(int id)
        {
            return null;
        }

        public Component GetPreviousComponent()
        {
            return componentsCache[previousComponentID];
        }

        public Component GetCurrentComponent()
        {
            return componentsCache[currentComponentID];
        }

        public Dictionary<string, int> GetCourses()
        {
            Dictionary<string, int> coursesDictionary = new Dictionary<string, int>();
            foreach (var course in courses)
            {
                coursesDictionary.Add(((Course)course).Name, course.ID);
            }
            return coursesDictionary;
        }
        
        public Component GetChildComponent(Component parent, ComponentType childType, EducationalLevelType educationalLevel,
            GradeType gradeNum)
        {
            previousComponentID = parent.ID;
            //check for educational level
            if (((Course)parent).Components.Exists(
                    x =>
                         x is EducationalLevel &&
                        ((EducationalLevel)x).Level == educationalLevel)) //exception on if
            {
                //found educational level now check for grade
                Component temp = ((Course) parent).Components.Find(x => ((EducationalLevel) x).Level == educationalLevel);
                if (((EducationalLevel)temp).Components.Exists(x => x is Grade && ((Grade)x).GradeNum == gradeNum))
                {
                    //found grade
                    currentComponentID = temp.ID;
                    return ((EducationalLevel) temp).Components.Find(x => ((Grade) x).GradeNum == gradeNum);
                }
                //create new grade
                Component newGrade = ContentFactory.CreateCompositeComponent(ComponentType.Grade, gradeNum);
                ((EducationalLevel)temp).Components.Add(newGrade);
                currentComponentID = newGrade.ID;
                return newGrade;
            }
            //create new educational level 
            Component eduLevel = ContentFactory.CreateCompositeComponent(ComponentType.EducationalLevel, educationalLevel);
            ((Course)parent).Components.Add(eduLevel);
            //create new grade
            Component grade = ContentFactory.CreateCompositeComponent(ComponentType.Grade, gradeNum);
            ((EducationalLevel)eduLevel).Components.Add(grade);
            currentComponentID = grade.ID;
            return grade;
        }

        #region ISubject

        public void Attach(IObserver observer)
        {
            throw new NotImplementedException();
        }

        public void Detach(IObserver observer)
        {
            throw new NotImplementedException();
        }

        public void Notify()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMigration

        public string Export()
        {
            throw new NotImplementedException();
        }

        public void Import(string content)
        {
            throw new NotImplementedException();
        }

        #endregion

        public Component GetCourse(string country)
        {
            return buildingCourse = courses.Exists(x => ((Course)x).Name == country) 
                ? courses.Find(x => ((Course)x).Name == country) 
                : ContentFactory.CreateCompositeComponent(ComponentType.Course);
        }

        public void CreateWeekComponents(Component buildingCourse, ComponentType componentType, ObservableCollection<Component> termI,
            ObservableCollection<Component> termII, ObservableCollection<Component> termIII)
        {
            foreach (Week week in termI)
            {
                ((Grade)buildingCourse).Components.Add(week);
            }
            foreach (Week week in termII)
            {
                ((Grade)buildingCourse).Components.Add(week);
            }
            foreach (Week week in termIII)
            {
                ((Grade)buildingCourse).Components.Add(week);
            }
        }

        public static int GetCurrentComponentID()
        {
            return currentComponentID;
        }

        public static int GetPreviousComponentID()
        {
            return previousComponentID;
        }

        public ObservableCollection<Week> GetAllWeeks(Component component)
        {
            ObservableCollection<Week> weeks = new ObservableCollection<Week>();
            if (component != null)
            {
                foreach (Week week in ((Grade)component).Components)
                {
                    weeks.Add(week);
                }
            }
            return weeks;
        }

        public ObservableCollection<Lesson> GetAllLessons(Component component)
        {
            ObservableCollection<Lesson> lessons = new ObservableCollection<Lesson>();
            foreach (Week week in ((Grade)component).Components)
            {
                foreach (Lesson lesson in week.Components)
                {
                    lessons.Add(lesson);
                }
            }
            return lessons;
        }

        public void SaveCourse()
        {
            courses.Add(buildingCourse);
            SaveManifest();
        }

        private void SaveManifest()
        {
            XmlSerializer serializer = new XmlSerializer(courses.GetType());
            serializer.Serialize(new XmlTextWriter("contentManifest.xml", Encoding.Unicode), courses);
        }
    }
}
