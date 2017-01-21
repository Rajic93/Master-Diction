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
using Microsoft.SqlServer.Server;

namespace Diction_Master___Library
{
    public class ContentManager : ISubject, IMigrate
    {
        private static ContentManager contentManager;
        private ClientManager clientManager;
        private static object _lock = new object();
        private static List<Component> courses { get; set; }
        private Dictionary<int, Component> componentsCache;
        private int _globalIDCounter;


        private Component buildingCourse;
        private static int previousComponentID;
        private static int currentComponentID;

        private ContentManager()
        {
            courses = new List<Component>();
            componentsCache = new Dictionary<int, Component>();
            clientManager = ClientManager.CreateInstance();
            _globalIDCounter = 1;
            LoadManifest();
        }

        public static ContentManager CreateInstance()
        {
            lock (_lock)
            {
                return contentManager ?? new ContentManager();
            }
        }

        private int GetID()
        {
            return _globalIDCounter++;
        }
        private void LoadManifest()
        {

        }

        private void SaveManifest()
        {
            XmlSerializer serializer = new XmlSerializer(courses.GetType());
            serializer.Serialize(new XmlTextWriter("contentManifest.xml", Encoding.Unicode), courses);
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

        #region potential delete

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

        public void SaveCourse()
        {
            courses.Add(buildingCourse);
            SaveManifest();
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
                    return new ContentFile() { ComponentType = ComponentType.Audio };
                case ComponentType.Video:
                    return new ContentFile() { ComponentType = ComponentType.Video };
                case ComponentType.Document:
                    return new ContentFile() { ComponentType = ComponentType.Document };
                default:
                    return null;
            }
        }

        //public Component GetChildComponent(Component parent, ComponentType childType, EducationalLevelType educationalLevel,
        //    GradeType gradeNum)
        //{
        //    previousComponentID = parent.ID;
        //    //check for educational level
        //    if (((Course)parent).Components.Exists(
        //            x =>
        //                 x is EducationalLevel &&
        //                ((EducationalLevel)x).Level == educationalLevel)) //exception on if
        //    {
        //        //found educational level now check for grade
        //        Component temp = ((Course) parent).Components.Find(x => ((EducationalLevel) x).Level == educationalLevel);
        //        if (((EducationalLevel)temp).Components.Exists(x => x is Grade && ((Grade)x).GradeNum == gradeNum))
        //        {
        //            //found grade
        //            currentComponentID = temp.ID;
        //            return ((EducationalLevel) temp).Components.Find(x => ((Grade) x).GradeNum == gradeNum);
        //        }
        //        //create new grade
        //        Component newGrade = ContentFactory.CreateCompositeComponent(ComponentType.Grade, gradeNum);
        //        ((EducationalLevel)temp).Components.Add(newGrade);
        //        currentComponentID = newGrade.ID;
        //        return newGrade;
        //    }
        //    //create new educational level 
        //    Component eduLevel = ContentFactory.CreateCompositeComponent(ComponentType.EducationalLevel, educationalLevel);
        //    ((Course)parent).Components.Add(eduLevel);
        //    //create new grade
        //    Component grade = ContentFactory.CreateCompositeComponent(ComponentType.Grade, gradeNum);
        //    ((EducationalLevel)eduLevel).Components.Add(grade);
        //    currentComponentID = grade.ID;
        //    return grade;
        //}

        //public static int GetCurrentComponentID()
        //{
        //    return currentComponentID;
        //}

        //public static int GetPreviousComponentID()
        //{
        //    return previousComponentID;
        //}

        #endregion

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

        public int GetNoOfWeeks(int id)
        {
            Grade grade = GetComponent(id)as Grade;
            return grade.Components.Count;
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

        public int GetNoOfLessons(int id)
        {
            return GetAllLessons(GetComponent(id)).Count;
        }

        public int GetNoOfFiles(int id)
        {
            int counter = 0;
            foreach (Lesson lesson in GetAllLessons(GetComponent(id)))
            {
                foreach (var child in lesson.Components)
                {
                    if (child.GetType().Name == "Quiz")
                        counter += (child as Quiz).Components.Count;
                    else
                        counter++;
                }
            }
            return counter;
        }

        private void AddComponent(int ID, Component component)
        {
            if (componentsCache.ContainsKey(ID))
            {
                Component temp = componentsCache[ID];
                (temp as CompositeComponent).Add(component);
            }
        }

        public Component GetComponent(int id)
        {
            if (componentsCache.ContainsKey(id))
            {
                return componentsCache[id];
            }
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

        public void SetCurrentComponent(int id)
        {
            if (componentsCache.ContainsKey(id))
            {
                previousComponentID = currentComponentID;
                currentComponentID = id;
            }
        }

        public ObservableCollection<Course> GetCourses()
        {
            //Dictionary<string, int> coursesDictionary = new Dictionary<string, int>();
            //foreach (var course in courses)
            //{
            //    coursesDictionary.Add(((Course)course).Name, course.ID);
            //}
            //return coursesDictionary;
            ObservableCollection<Course> coursesCollection = new ObservableCollection<Course>();
            foreach (Course course in courses)
            {
                coursesCollection.Add(course);
            }
            return coursesCollection;
        }

        public List<int> GetChildrenIDs(int id)
        {
            if (componentsCache.ContainsKey(id))
            {
                List<int> children = new List<int>();
                CompositeComponent component = componentsCache[id] as CompositeComponent;
                foreach (Component childComponent in component.Components)
                {
                    children.Add(childComponent.ID);
                }
                return children;
            }
            return null;
        }

        #region Composite components

        #region Course

        public int AddCourse(string name, string iconURI)
        {
            if (courses != null)
            {
                if (!courses.Exists(x => ((Course)x).Name== name || ((Course)x).Icon == iconURI))
                {
                    int id = GetID();
                    Course course = ContentFactory.CreateCompositeComponent(id, 0, name, iconURI) as Course;
                    componentsCache[id] = course;
                    courses.Add(course);
                    return id;
                }
                return 0;
                //log message if course exist
            }
            return -1;
            //log message if courses is null
        }

        public void EditCourse(int id, string name, string iconURI)
        {
            if (courses != null)
            {
                if (componentsCache.ContainsKey(id))
                {
                    Course course = componentsCache[id] as Course;
                    course.Name = name ?? course.Name;
                    course.Icon = iconURI ?? course.Icon;
                }
                //course does not exist
            }
            //courses list is null
        }

        public void DeleteCourse(int id)
        {
            if (courses != null)
            {
                if (courses.Exists(x=> x.ID == id))
                {
                    int index = courses.FindIndex(x => x.ID == id);
                    courses.RemoveAt(index);
                    componentsCache.Remove(id);
                }
                //course does not exist
            }
            //courses list is null
        }

        #endregion

        #region EducationalLevel

        public int AddEducationalLevel(int parentID, string iconURI, EducationalLevelType type)
        {
            if (componentsCache.ContainsKey(parentID))//check for course
            {
                EducationalLevel level = (componentsCache[parentID] as Course).FindEducationLevel(type);
                if (level == null)//there is no existing edu level
                {
                    int ID = GetID();
                    level = ContentFactory.CreateCompositeComponent(ID, parentID, type, iconURI) as EducationalLevel;
                    (componentsCache[parentID] as Course).Add(level);
                    componentsCache[ID] = level;
                    return ID;
                }
                return 0;
            }
            return -1;
        }

        public void EditEducationalLevel(int id, string iconURI, EducationalLevelType levelType)
        {
            if (componentsCache != null && componentsCache.ContainsKey(id))
            {
                EducationalLevel level = componentsCache[id] as EducationalLevel;
                if (level != null)
                {
                    level.Icon = iconURI ?? level.Icon;
                    level.Level = levelType;
                }
            }
        }

        public void DeleteEducationalLevel(int id, int parentID)
        {
            if (componentsCache.ContainsKey(parentID) && componentsCache.ContainsKey(id))
            {
                (componentsCache[parentID] as Course).Components.RemoveAll(x => x.ID == id);
                componentsCache.Remove(id);
            }
        }
        #endregion

        #region Grade

        public int AddGrade(int parentID, string icon, GradeType type)
        {
            if (componentsCache.ContainsKey(parentID))//check for course
            {
                if (componentsCache[parentID] is EducationalLevel)
                {
                    Grade grade = (componentsCache[parentID] as EducationalLevel).FindGrade(type);
                    if (grade == null)//there is no existing edu level
                    {
                        int ID = GetID();
                        grade = ContentFactory.CreateCompositeComponent(ID, parentID, icon, type) as Grade;
                        (componentsCache[parentID] as EducationalLevel).Add(grade);
                        componentsCache[ID] = grade;
                        return ID;
                    }
                }
                return 0;
                //edu level exists
            }
            return -1;
            //there is no such edu level
        }

        public void EditGrade(int id, string icon, GradeType type)
        {
            if (componentsCache.ContainsKey(id))
            {
                Grade grade = componentsCache[id] as Grade;
                grade.GradeNum = type;
                grade.Icon = icon ?? grade.Icon;
            }
        }

        public void DeleteGrade(int id, int parentID)
        {
            if (componentsCache.ContainsKey(id) && componentsCache.ContainsKey(parentID))
            {
                (componentsCache[parentID] as EducationalLevel).Components.RemoveAll(x => x.ID == id);
                componentsCache.Remove(id);
            }
        }

        #endregion

        #region Week

        public int AddWeek(int parentID, string title, int num, int term)
        {
            if (componentsCache.ContainsKey(parentID))
            {
                Week week = (componentsCache[parentID] as Grade).FindWeek(title, num, term);
                if (week == null)
                {
                    int ID = GetID();
                    week = ContentFactory.CreateCompositeComponent(ID, parentID, title, num, term) as Week;
                    componentsCache[ID] = week;
                    (componentsCache[parentID] as Grade).Components.Add(week);
                    return ID;
                }
                return 0;
                //there is such week
            }
            return -1;
            //there is no such parent grade
        }

        public void EditWeek(int id, string title, int num, int term)
        {
            if (componentsCache.ContainsKey(id))
            {
                Week week = componentsCache[id] as Week;
                week.Num = num;
                week.Term = term;
                week.Title = title ?? week.Title;
            }
            //there is such week
        }

        public void DeleteWeek(int id, int parentID)
        {
            if (componentsCache.ContainsKey(id) && componentsCache.ContainsKey(parentID))
            {
                (componentsCache[parentID] as Grade).Components.RemoveAll(x => x.ID == id);
                componentsCache.Remove(id);
            }
            //there is such week
        }

        #endregion

        #region Lesson

        public void AddLesson(int parentID, string title, int num)
        {
            if (componentsCache.ContainsKey(parentID))
            {
                Lesson lesson = (componentsCache[parentID] as Week).FindLesson(title, num);
                if (lesson == null)
                {
                    int ID = GetID();
                    lesson = ContentFactory.CreateCompositeComponent(ID, parentID, title, num) as Lesson;
                    componentsCache[ID] = lesson;
                    (componentsCache[parentID] as Week).Components.Add(lesson);
                }
            }
        }

        public void EditLesson(int id, string title, int num)
        {
            if (componentsCache.ContainsKey(id))
            {
                Lesson lesson = componentsCache[id] as Lesson;
                lesson.Num = num;
                lesson.Title = title ?? lesson.Title;
            }
        }

        public void DeleteLesson(int id, int parentID)
        {
            if (componentsCache.ContainsKey(id) && componentsCache.ContainsKey(parentID))
            {
                (componentsCache[parentID] as Week).Components.RemoveAll(x => (x as Lesson).ID == id);
                componentsCache.Remove(id);
            }
        }

        #endregion

        #region Quiz

        public void AddQuiz(int parentID, string title)
        {
            if (componentsCache.ContainsKey(parentID))
            {
                Quiz quiz = (componentsCache[parentID] as Lesson).FindQuiz(title);
                if (quiz == null)
                {
                    int id = GetID();
                    quiz = ContentFactory.CreateCompositeComponent(id, parentID, title) as Quiz;
                    (componentsCache[parentID] as Lesson).Components.Add(quiz);
                    componentsCache[id] = quiz;
                }
            }
        }

        public void EditQuiz(int id, string title)
        {
            if (componentsCache.ContainsKey(id))
            {
                Quiz quiz = componentsCache[id] as Quiz;
                quiz.Title = title ?? quiz.Title;
            }
        }

        public void DeleteQuiz(int id, int parentID)
        {
            if (componentsCache.ContainsKey(id) && componentsCache.ContainsKey(parentID))
            {
                (componentsCache[parentID] as Lesson).Components.RemoveAll(x => (x as Quiz).ID == id);
                componentsCache.Remove(id);
            }
        }

        #endregion

        #endregion

        #region Leaf components

        #region ContentFile

        public void AddContentFile(int parentID, ComponentType type, string icon,
            string title, string uri, float size, string description)
        {
            if (componentsCache.ContainsKey(parentID))
            {
                ContentFile file = (componentsCache[parentID] as Lesson).FindContentFile(type, uri, size);
                if (file == null)
                {
                    int id = GetID();
                    file = ContentFactory.CreateLeafComponent(id, parentID, type, title, uri, size, description, icon) as ContentFile;
                    (componentsCache[parentID] as Lesson).Components.Add(file);
                    componentsCache[id] = file;
                }
            }
        }

        public void EditContentFile(int id, ComponentType type, string icon,
            string title, string uri, float size, string description)
        {
            if (componentsCache.ContainsKey(id))
            {
                ContentFile file = componentsCache[id] as ContentFile;
                file.ComponentType = type;
                file.icon = icon ?? file.icon;
                file.Title = title ?? file.Title;
                file.URI = uri ?? file.URI;
                file.Size = size;
                file.Description = description ?? file.Description;
            }
        }

        public void DeleteContentFile(int id, int parentID)
        {
            if (componentsCache.ContainsKey(id) && componentsCache.ContainsKey(parentID))
            {
                (componentsCache[parentID] as Lesson).Components.RemoveAll(x => x.ID == id);
                componentsCache.Remove(id);
            }
        }

        #endregion

        #region Question

        public void AddQuestion(int parentID, string text, string answer, QuestionType type)
        {
            if (componentsCache.ContainsKey(parentID))
            {
                Question file = (componentsCache[parentID] as Quiz).FindQuestion(text, answer);
                if (file == null)
                {
                    int id = GetID();
                    file = ContentFactory.CreateLeafComponent(id, parentID, text, answer, type) as Question;
                    (componentsCache[parentID] as Quiz).Components.Add(file);
                    componentsCache[id] = file;
                }
            }
        }

        public void EditQuestion(int id, string text, string answer, QuestionType type)
        {
            if (componentsCache.ContainsKey(id))
            {
                Question question = componentsCache[id] as Question;
                question.Text = text ?? question.Text;
                question.Answer = answer ?? question.Answer;
                question.Type = type;
            }
        }

        public void DeleteQuestion(int id, int parentID)
        {
            if (componentsCache.ContainsKey(id) && componentsCache.ContainsKey(parentID))
            {
                (componentsCache[parentID] as Quiz).Components.RemoveAll(x => x.ID == id);
                componentsCache.Remove(id);
            }
        }

        #endregion

        #endregion
    }
}
