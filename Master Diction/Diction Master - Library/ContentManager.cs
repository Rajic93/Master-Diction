using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
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
    /// <summary>
    /// 
    /// </summary>
    public sealed class ContentManager : ISubject, IMigrate
    {
        /// <summary>
        /// 
        /// </summary>
        private static ContentManager _contentManager;
        /// <summary>
        /// 
        /// </summary>
        private ClientManager clientManager;
        /// <summary>
        /// 
        /// </summary>
        private static readonly object Lock = new object();
        /// <summary>
        /// 
        /// </summary>
        private List<Component> courses { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<int, Component> _componentsCache;
        /// <summary>
        /// 
        /// </summary>
        private int _globalIdCounter;
        /// <summary>
        /// 
        /// </summary>
        private ContentManager()
        {
            courses = new List<Component>();
            _componentsCache = new Dictionary<int, Component>();
            clientManager = ClientManager.CreateInstance();
            _globalIdCounter = 1;
            LoadManifest();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ContentManager CreateInstance()
        {
            if (_contentManager == null)
            {
                lock (Lock)
                {
                    if (_contentManager == null)
                    {
                        return new ContentManager();
                    }
                    return _contentManager;
                }
            }
            return _contentManager;
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadManifest()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        private void SaveManifest()
        {
            XmlSerializer serializer = new XmlSerializer(courses.GetType());
            serializer.Serialize(new XmlTextWriter("contentManifest.xml", Encoding.Unicode), courses);
        }
        /// <summary>
        /// 
        /// </summary>
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

        #region Get functions
        
        private int GetID()
        {
            return _globalIdCounter++;
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

        public int GetNoOfWeeks(int id)
        {
            Grade grade = GetComponent(id) as Grade;
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

        public ObservableCollection<Question> GetAllQuestions(int parent)
        {
            ObservableCollection<Question> questions = new ObservableCollection<Question>();
            Quiz quiz = GetComponent(parent) as Quiz;
            if (quiz != null)
            {
                foreach (Question question in quiz.Components)
                {
                    questions.Add(question);
                }
            }
            return questions;
        }

        public Component GetComponent(int id)
        {
            if (_componentsCache.ContainsKey(id))
            {
                return _componentsCache[id];
            }
            return null;
        }

        public List<int> GetChildrenIDs(int id)
        {
            if (_componentsCache.ContainsKey(id))
            {
                List<int> children = new List<int>();
                CompositeComponent component = _componentsCache[id] as CompositeComponent;
                foreach (Component childComponent in component.Components)
                {
                    children.Add(childComponent.ID);
                }
                return children;
            }
            return null;
        }

        #endregion

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
                    _componentsCache[id] = course;
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
                if (_componentsCache.ContainsKey(id))
                {
                    Course course = _componentsCache[id] as Course;
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
                    _componentsCache.Remove(id);
                }
                //course does not exist
            }
            //courses list is null
        }

        #endregion

        #region EducationalLevel

        public int AddEducationalLevel(int parentID, string iconURI, EducationalLevelType type)
        {
            if (_componentsCache.ContainsKey(parentID))//check for course
            {
                EducationalLevel level = (_componentsCache[parentID] as Course).FindEducationLevel(type);
                if (level == null)//there is no existing edu level
                {
                    int ID = GetID();
                    level = ContentFactory.CreateCompositeComponent(ID, parentID, type, iconURI) as EducationalLevel;
                    (_componentsCache[parentID] as Course).Add(level);
                    _componentsCache[ID] = level;
                    return ID;
                }
                return 0;
            }
            return -1;
        }

        public void EditEducationalLevel(int id, string iconURI, EducationalLevelType levelType)
        {
            if (_componentsCache != null && _componentsCache.ContainsKey(id))
            {
                EducationalLevel level = _componentsCache[id] as EducationalLevel;
                if (level != null)
                {
                    level.Icon = iconURI ?? level.Icon;
                    level.Level = levelType;
                }
            }
        }

        public void DeleteEducationalLevel(int id, int parentID)
        {
            if (_componentsCache.ContainsKey(parentID) && _componentsCache.ContainsKey(id))
            {
                (_componentsCache[parentID] as Course).Components.RemoveAll(x => x.ID == id);
                _componentsCache.Remove(id);
            }
        }
        #endregion

        #region Grade

        public int AddGrade(int parentID, string icon, GradeType type)
        {
            if (_componentsCache.ContainsKey(parentID))//check for course
            {
                if (_componentsCache[parentID] is EducationalLevel)
                {
                    Grade grade = (_componentsCache[parentID] as EducationalLevel).FindGrade(type);
                    if (grade == null)//there is no existing grade
                    {
                        int ID = GetID();
                        grade = ContentFactory.CreateCompositeComponent(ID, parentID, icon, type) as Grade;
                        (_componentsCache[parentID] as EducationalLevel).Add(grade);
                        _componentsCache[ID] = grade;
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
            if (_componentsCache.ContainsKey(id))
            {
                Grade grade = _componentsCache[id] as Grade;
                grade.GradeNum = type;
                grade.Icon = icon ?? grade.Icon;
            }
        }

        public void DeleteGrade(int id, int parentID)
        {
            if (_componentsCache.ContainsKey(id) && _componentsCache.ContainsKey(parentID))
            {
                (_componentsCache[parentID] as EducationalLevel).Components.RemoveAll(x => x.ID == id);
                _componentsCache.Remove(id);
            }
        }

        #endregion

        #region Week

        public int AddWeek(int parentID, string title, int num, int term)
        {
            if (_componentsCache.ContainsKey(parentID))
            {
                Week week = (_componentsCache[parentID] as Grade).FindWeek(title, num, term);
                if (week == null)
                {
                    int ID = GetID();
                    week = ContentFactory.CreateCompositeComponent(ID, parentID, title, num, term) as Week;
                    _componentsCache[ID] = week;
                    (_componentsCache[parentID] as Grade).Components.Add(week);
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
            if (_componentsCache.ContainsKey(id))
            {
                Week week = _componentsCache[id] as Week;
                week.Num = num;
                week.Term = term;
                week.Title = title ?? week.Title;
            }
            //there is such week
        }

        public void DeleteWeek(int id, int parentID)
        {
            if (_componentsCache.ContainsKey(id) && _componentsCache.ContainsKey(parentID))
            {
                (_componentsCache[parentID] as Grade).Components.RemoveAll(x => x.ID == id);
                _componentsCache.Remove(id);
            }
            //there is such week
        }

        #endregion

        #region Lesson

        public int AddLesson(int parentID, string title, int num)
        {
            if (_componentsCache.ContainsKey(parentID))
            {
                Lesson lesson = (_componentsCache[parentID] as Week).FindLesson(title, num);
                if (lesson == null)
                {
                    int ID = GetID();
                    lesson = ContentFactory.CreateCompositeComponent(ID, parentID, title, num) as Lesson;
                    _componentsCache[ID] = lesson;
                    (_componentsCache[parentID] as Week).Components.Add(lesson);
                    return ID;
                }
                return 0;
            }
            return -1;
        }

        public void EditLesson(int id, string title, int num)
        {
            if (_componentsCache.ContainsKey(id))
            {
                Lesson lesson = _componentsCache[id] as Lesson;
                lesson.Num = num;
                lesson.Title = title ?? lesson.Title;
            }
        }

        public void DeleteLesson(int id, int parentID)
        {
            if (_componentsCache.ContainsKey(id) && _componentsCache.ContainsKey(parentID))
            {
                (_componentsCache[parentID] as Week).Components.RemoveAll(x => (x as Lesson).ID == id);
                _componentsCache.Remove(id);
            }
        }

        #endregion

        #region Quiz

        public int AddQuiz(int parentID, string title)
        {
            if (_componentsCache.ContainsKey(parentID))
            {
                Quiz quiz = (_componentsCache[parentID] as Lesson).FindQuiz(title);
                if (quiz == null)
                {
                    int id = GetID();
                    quiz = ContentFactory.CreateCompositeComponent(id, parentID, title) as Quiz;
                    (_componentsCache[parentID] as Lesson).Components.Add(quiz);
                    _componentsCache[id] = quiz;
                    return id;
                }
                return 0;
            }
            return -1;
        }

        public void EditQuiz(int id, string title)
        {
            if (_componentsCache.ContainsKey(id))
            {
                Quiz quiz = _componentsCache[id] as Quiz;
                quiz.Title = title ?? quiz.Title;
            }
        }

        public void DeleteQuiz(int id, int parentID)
        {
            if (_componentsCache.ContainsKey(id) && _componentsCache.ContainsKey(parentID))
            {
                (_componentsCache[parentID] as Lesson).Components.RemoveAll(x => x.ID == id);
                _componentsCache.Remove(id);
            }
        }

        #endregion

        #endregion

        #region Leaf components

        #region ContentFile

        public int AddContentFile(int parentID, ComponentType type, string icon,
            string title, string uri, long size, string description)
        {
            if (_componentsCache.ContainsKey(parentID))
            {
                ContentFile file = (_componentsCache[parentID] as Lesson).FindContentFile(type, uri, size);
                if (file == null)
                {
                    int id = GetID();
                    file = ContentFactory.CreateLeafComponent(id, parentID, type, title, uri, size, description, icon) as ContentFile;
                    (_componentsCache[parentID] as Lesson).Components.Add(file);
                    _componentsCache[id] = file;
                    return id;
                }
                return 0;
            }
            return -1;
        }

        public void EditContentFile(int id, ComponentType type, string icon,
            string title, string uri, long size, string description)
        {
            if (_componentsCache.ContainsKey(id))
            {
                ContentFile file = _componentsCache[id] as ContentFile;
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
            if (_componentsCache.ContainsKey(id) && _componentsCache.ContainsKey(parentID))
            {
                (_componentsCache[parentID] as Lesson).Components.RemoveAll(x => x.ID == id);
                _componentsCache.Remove(id);
            }
        }

        #endregion

        #region Question

        public int AddQuestion(int parentID, string text, string answer, QuestionType type, ObservableCollection<string> wrongAnswers)
        {
            if (_componentsCache.ContainsKey(parentID))
            {
                Question file = (_componentsCache[parentID] as Quiz).FindQuestion(text, answer);
                if (file == null)
                {
                    int id = GetID();
                    if (type != QuestionType.Choice)
                        file = ContentFactory.CreateLeafComponent(id, parentID, text, answer, type) as Question;
                    else 
                        //mozda u factory treba da se promeni lista
                        file = ContentFactory.CreateLeafComponent(id, parentID, text, answer, type, wrongAnswers) as Question;
                    (_componentsCache[parentID] as Quiz).Components.Add(file);
                    _componentsCache[id] = file;
                    return id;
                }
                return 0;
            }
            return -1;
        }

        public void EditQuestion(int id, string text, string answer, QuestionType type, ObservableCollection<string> wrongAnswers)
        {
            if (_componentsCache.ContainsKey(id))
            {
                Question question = _componentsCache[id] as Question;
                question.Text = text ?? question.Text;
                question.Answer = answer ?? question.Answer;
                question.Type = type;
                question.WrongAnswers = wrongAnswers;
            }
        }

        public void DeleteQuestion(int id, int parentID)
        {
            if (_componentsCache.ContainsKey(id) && _componentsCache.ContainsKey(parentID))
            {
                (_componentsCache[parentID] as Quiz).Components.RemoveAll(x => x.ID == id);
                _componentsCache.Remove(id);
            }
        }

        #endregion

        private void SaveFile(string filePath)
        {
            //Directory.SetCurrentDirectory("tmp\\");
            //File.WriteAllBytes(name, bytes);
        }

        public string CreateNewArchive(string userId, string action)
        {
            string archiveName = "";
            try
            {
                string dirPath = ""; //this.GetDirPath(userId, action);
                //long currentTime = (Int32)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                //string timestamp = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
                FileInfo testExist = new FileInfo(System.IO.Path.Combine(dirPath, userId + ".zip"));
                //archiveName = ;
                while (testExist.Exists)
                {
                    //currentTime = (Int32)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    testExist = new FileInfo(System.IO.Path.Combine(dirPath, userId + ".zip"));
                }
                archiveName = System.IO.Path.Combine(dirPath, userId + ".zip");
                using (System.IO.File.Create(archiveName))
                {

                }
            }
            catch (Exception ex)
            {
                return "ERROR";
            }
            return archiveName;
        }

        #endregion
    }
}
