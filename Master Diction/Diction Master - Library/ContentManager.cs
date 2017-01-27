using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
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
        private List<Component> Courses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private List<Component> Topics { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<long, Component> ComponentsCache;

        /// <summary>
        /// 
        /// </summary>
        private long _globalIdCounter;
        /// <summary>
        /// 
        /// </summary>
        private readonly string _encryptionKey = "pfKLNfYgJG6CWi46fyFzXpyr";
        /// <summary>
        /// 
        /// </summary>
        private List<ContentVersionInfo> ChangesDiction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private List<ContentVersionInfo> ChangesTeachers { get; set; }

        private ApplicationType _type;

        /// <summary>
        /// 
        /// </summary>
        private ContentManager()
        {
            Courses = new List<Component>();
            Topics = new List<Component>();
            ComponentsCache = new Dictionary<long, Component>();
            clientManager = ClientManager.CreateInstance();
            ChangesDiction = new List<ContentVersionInfo>();
            ChangesTeachers = new List<ContentVersionInfo>();
            _globalIdCounter = 1;
            LoadManifest();
            SetUpCache(Courses);
            SetUpCache(Topics);
        }

        private void SetUpCache(List<Component> components)
        {
            foreach (Component component in components)
            {
                ComponentsCache[component.ID] = component;
                if (component is CompositeComponent)
                {
                    SetUpCache((component as CompositeComponent).Components);
                }
            }
        }

        ~ContentManager()
        {
            SaveManifest();
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
            XmlSerializer serializer = new XmlSerializer(Courses.GetType());
            if (File.Exists("DictionAppContentManifest.xml"))
                Courses = serializer.Deserialize(new XmlTextReader("DictionAppContentManifest.xml")) as List<Component>;
            if (File.Exists("TeachersAppContentManifest.xml"))
            Topics = serializer.Deserialize(new XmlTextReader("TeachersAppContentManifest.xml")) as List<Component>;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveManifest()
        {

            XmlSerializer serializer = new XmlSerializer(Courses.GetType());
            serializer.Serialize(new XmlTextWriter("DictionAppContentManifest.xml", Encoding.Unicode), Courses);
            serializer.Serialize(new XmlTextWriter("TeachersAppContentManifest.xml", Encoding.Unicode), Topics);
            serializer = new XmlSerializer(GetType());
            serializer.Serialize(new XmlTextWriter("ContentManifest.xml", Encoding.Unicode), this);
        }

        /// <summary>
        /// 
        /// </summary>
        public Thread Start()
        {
            Thread thread = new Thread(() =>
            {
                while (true)
                {

                }
            });
            thread.Start();
            return thread;
        }

        public void SetAppType(ApplicationType type)
        {
            _type = type;
        }

        #region ISubject

        public void Attach(IObserver observer)
        {
            clientManager = observer as ClientManager;
        }

        public void Detach(IObserver observer)
        {
            
        }

        public void Notify(ApplicationType type)
        {
            switch (type)
            {
                    case ApplicationType.Audio:
                    break;
                    case ApplicationType.Diction:
                    clientManager.Update(ChangesDiction);
                    break;
                    case ApplicationType.Teachers:
                    clientManager.Update(ChangesTeachers);
                    break;
            }
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

        private long GetID()
        {
            return _globalIdCounter++;
        }

        public List<Course> GetAllCourses()
        {
            List<Course> coursesList = new List<Course>();
            foreach (Course course in Courses)
            {
                coursesList.Add(course);
            }
            return coursesList;
        }

        public int GetNoOfCourses()
        {
            return Courses.Count;
        }

        public ObservableCollection<Component> GetAllWeeks(Component component)
        {
            ObservableCollection<Component> weeks = new ObservableCollection<Component>();
            if (component != null)
            {
                foreach (Week week in ((Grade) component).Components)
                {
                    weeks.Add(week);
                }
            }
            return weeks;
        }

        public int GetNoOfWeeks(long id)
        {
            Grade grade = GetComponent(id) as Grade;
            return grade.Components.Count;
        }

        public ObservableCollection<Component> GetAllTopics()
        {
            ObservableCollection<Component> _topics = new ObservableCollection<Component>();
            foreach (Topic topic in Topics)
            {
                _topics.Add(topic);
            }
            return _topics;
        }

        public int GetNoOfTopicsLessons()
        {
            int num = GetAllTopicsLessons().Count;
            return num;
        }

        public ObservableCollection<Lesson> GetAllLessons(Component component)
        {
            ObservableCollection<Lesson> lessons = new ObservableCollection<Lesson>();
            foreach (Week week in ((CompositeComponent) component).Components)
            {
                foreach (Lesson lesson in week.Components)
                {
                    lessons.Add(lesson);
                }
            }
            return lessons;
        }

        public ObservableCollection<Lesson> GetAllTopicsLessons()
        {
            ObservableCollection<Lesson> lessons = new ObservableCollection<Lesson>();
            foreach (CompositeComponent topic in Topics)
            {
                foreach (Lesson lesson in topic.Components)
                {
                    lessons.Add(lesson);
                }
            }
            return lessons;
        }

        public int GetNoOfLessons(long id)
        {
            return GetAllLessons(GetComponent(id)).Count;
        }

        public int GetNoOfFiles(long id)
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

        public ObservableCollection<Question> GetAllQuestions(long parent)
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

        public Component GetComponent(long id)
        {
            if (ComponentsCache.ContainsKey(id))
            {
                return ComponentsCache[id];
            }
            return null;
        }

        public List<long> GetChildrenIDs(long id)
        {
            if (ComponentsCache.ContainsKey(id))
            {
                List<long> children = new List<long>();
                CompositeComponent component = ComponentsCache[id] as CompositeComponent;
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

        public long AddCourse(string name, string iconURI)
        {
            if (Courses != null)
            {
                if (!Courses.Exists(x => ((Course) x).Name == name || ((Course) x).Icon == iconURI))
                {
                    long id = GetID();
                    Course course = ContentFactory.CreateCompositeComponent(id, 0, name, iconURI) as Course;
                    ComponentsCache[id] = course;
                    Courses.Add(course);
                    return id;
                }
                return 0;
                //log message if course exist
            }
            return -1;
            //log message if courses is null
        }

        public void EditCourse(long id, string name, string iconURI)
        {
            if (Courses != null)
            {
                if (ComponentsCache.ContainsKey(id))
                {
                    Course course = ComponentsCache[id] as Course;
                    course.Name = name ?? course.Name;
                    course.Icon = iconURI ?? course.Icon;
                }
                //course does not exist
            }
            //courses list is null
        }

        public void DeleteCourse(long id)
        {
            if (Courses != null)
            {
                if (Courses.Exists(x => x.ID == id))
                {
                    RecursiveDelete(Courses.Find(x => x.ID == id));
                    int index = Courses.FindIndex(x => x.ID == id);
                    Courses.RemoveAt(index);
                    ComponentsCache.Remove(id);
                }
                //course does not exist
            }
            //courses list is null
        }

        #endregion

        #region Topic

        public long AddTopic(string title, int num)
        {
            if (Topics != null)
            {
                if (!Topics.Exists(x => ((Topic)x).Title == title || ((Topic)x).Num == num))
                {
                    long id = GetID();
                    Topic topic = ContentFactory.CreateCompositeComponent(id, 0, title, num, true);
                    ComponentsCache[id] = topic;
                    Topics.Add(topic);
                    return id;
                }
                return 0;
                //log message if course exist
            }
            return -1;
            //log message if courses is null
        }

        public void EditTopic(long id, string title, int num)
        {
            if (Topics != null)
            {
                if (ComponentsCache.ContainsKey(id))
                {
                    Topic topic = ComponentsCache[id] as Topic;
                    topic.Title = title;
                    topic.Num = num;
                }
                //course does not exist
            }
            //courses list is null
        }

        public void DeleteTopic(long id)
        {
            if (Topics != null)
            {
                if (Topics.Exists(x => x.ID == id))
                {
                    RecursiveDelete(Topics.Find(x => x.ID == id));
                    int index = Topics.FindIndex(x => x.ID == id);
                    Topics.RemoveAt(index);
                    ComponentsCache.Remove(id);
                }
                //course does not exist
            }
            //courses list is null
        }

        #endregion

        #region EducationalLevel

        public long AddEducationalLevel(long parentID, string iconURI, EducationalLevelType type)
        {
            if (ComponentsCache.ContainsKey(parentID)) //check for course
            {
                EducationalLevel level = (ComponentsCache[parentID] as Course).FindEducationLevel(type);
                if (level == null) //there is no existing edu level
                {
                    long ID = GetID();
                    level = ContentFactory.CreateCompositeComponent(ID, parentID, type, iconURI) as EducationalLevel;
                    (ComponentsCache[parentID] as Course).Add(level);
                    ComponentsCache[ID] = level;
                    return ID;
                }
                return 0;
            }
            return -1;
        }

        public void EditEducationalLevel(long id, string iconURI, EducationalLevelType levelType)
        {
            if (ComponentsCache != null && ComponentsCache.ContainsKey(id))
            {
                EducationalLevel level = ComponentsCache[id] as EducationalLevel;
                if (level != null)
                {
                    level.Icon = iconURI ?? level.Icon;
                    level.Level = levelType;
                }
            }
        }

        public void DeleteEducationalLevel(long id, long parentID)
        {
            if (ComponentsCache.ContainsKey(parentID) && ComponentsCache.ContainsKey(id))
            {
                RecursiveDelete((ComponentsCache[parentID] as Course).Components.Find(x => x.ID == id));
                (ComponentsCache[parentID] as Course).Components.RemoveAll(x => x.ID == id);
                ComponentsCache.Remove(id);
            }
        }

        #endregion

        #region Grade

        public long AddGrade(long parentID, string icon, GradeType type)
        {
            if (ComponentsCache.ContainsKey(parentID)) //check for course
            {
                if (ComponentsCache[parentID] is EducationalLevel)
                {
                    Grade grade = (ComponentsCache[parentID] as EducationalLevel).FindGrade(type);
                    if (grade == null) //there is no existing grade
                    {
                        long ID = GetID();
                        grade = ContentFactory.CreateCompositeComponent(ID, parentID, icon, type) as Grade;
                        (ComponentsCache[parentID] as EducationalLevel).Add(grade);
                        ComponentsCache[ID] = grade;
                        return ID;
                    }
                }
                return 0;
                //edu level exists
            }
            return -1;
            //there is no such edu level
        }

        public void EditGrade(long id, string icon, GradeType type)
        {
            if (ComponentsCache.ContainsKey(id))
            {
                Grade grade = ComponentsCache[id] as Grade;
                grade.GradeNum = type;
                grade.Icon = icon ?? grade.Icon;
            }
        }

        public void DeleteGrade(long id, long parentID)
        {
            if (ComponentsCache.ContainsKey(id) && ComponentsCache.ContainsKey(parentID))
            {
                RecursiveDelete((ComponentsCache[parentID] as EducationalLevel).Components.Find(x => x.ID == id));
                (ComponentsCache[parentID] as EducationalLevel).Components.RemoveAll(x => x.ID == id);
                ComponentsCache.Remove(id);
            }
        }

        #endregion

        #region Week

        public long AddWeek(long parentID, string title, int num, int term)
        {
            if (ComponentsCache.ContainsKey(parentID))
            {
                Week week = (ComponentsCache[parentID] as Grade).FindWeek(title, num, term);
                if (week == null)
                {
                    long ID = GetID();
                    week = ContentFactory.CreateCompositeComponent(ID, parentID, title, num, term) as Week;
                    ComponentsCache[ID] = week;
                    (ComponentsCache[parentID] as Grade).Components.Add(week);
                    return ID;
                }
                return 0;
                //there is such week
            }
            return -1;
            //there is no such parent grade
        }

        public void EditWeek(long id, string title, int num, int term)
        {
            if (ComponentsCache.ContainsKey(id))
            {
                Week week = ComponentsCache[id] as Week;
                week.Num = num;
                week.Term = term;
                week.Title = title ?? week.Title;
            }
            //there is such week
        }

        public void DeleteWeek(long id, long parentID)
        {
            if (ComponentsCache.ContainsKey(id) && ComponentsCache.ContainsKey(parentID))
            {
                RecursiveDelete((ComponentsCache[parentID] as Grade).Components.Find(x => x.ID == id));
                (ComponentsCache[parentID] as Grade).Components.RemoveAll(x => x.ID == id);
                ComponentsCache.Remove(id);
            }
            //there is such week
        }

        #endregion

        #region Lesson

        public long AddLesson(long parentID, string title, int num)
        {
            if (ComponentsCache.ContainsKey(parentID))
            {
                Lesson lesson = null;
                if (ComponentsCache[parentID].GetType().Name == "Week")
                    lesson = (ComponentsCache[parentID] as Week).FindLesson(title, num);
                else if (ComponentsCache[parentID].GetType().Name == "Topic")
                    lesson = (ComponentsCache[parentID] as Topic).FindLesson(title, num);
                if (lesson == null)
                {
                    long ID = GetID();
                    lesson = ContentFactory.CreateCompositeComponent(ID, parentID, title, num) as Lesson;
                    ComponentsCache[ID] = lesson;
                    (ComponentsCache[parentID] as CompositeComponent).Components.Add(lesson);
                    return ID;
                }
                return 0;
            }
            return -1;
        }

        public void EditLesson(long id, string title, int num)
        {
            if (ComponentsCache.ContainsKey(id))
            {
                Lesson lesson = ComponentsCache[id] as Lesson;
                lesson.Num = num;
                lesson.Title = title ?? lesson.Title;
            }
        }

        public void DeleteLesson(long id, long parentID)
        {
            if (ComponentsCache.ContainsKey(id) && ComponentsCache.ContainsKey(parentID))
            {
                RecursiveDelete((ComponentsCache[parentID] as CompositeComponent).Components.Find(x => x.ID == id));
                (ComponentsCache[parentID] as CompositeComponent).Components.RemoveAll(x => x.ID == id);
                ComponentsCache.Remove(id);
            }
        }

        #endregion

        #region Quiz

        public long AddQuiz(long parentID, string title)
        {
            if (ComponentsCache.ContainsKey(parentID))
            {
                Quiz quiz = (ComponentsCache[parentID] as Lesson).FindQuiz(title);
                if (quiz == null)
                {
                    long id = GetID();
                    quiz = ContentFactory.CreateCompositeComponent(id, parentID, title) as Quiz;
                    (ComponentsCache[parentID] as Lesson).Components.Add(quiz);
                    ComponentsCache[id] = quiz;
                    return id;
                }
                return 0;
            }
            return -1;
        }

        public void EditQuiz(long id, string title)
        {
            if (ComponentsCache.ContainsKey(id))
            {
                Quiz quiz = ComponentsCache[id] as Quiz;
                quiz.Title = title ?? quiz.Title;
            }
        }

        public void DeleteQuiz(long id, long parentID)
        {
            if (ComponentsCache.ContainsKey(id) && ComponentsCache.ContainsKey(parentID))
            {
                RecursiveDelete((ComponentsCache[parentID] as Lesson).Components.Find(x => x.ID == id));
                (ComponentsCache[parentID] as Lesson).Components.RemoveAll(x => x.ID == id);
                ComponentsCache.Remove(id);
            }
        }

        #endregion

        #endregion

        #region Leaf components

        #region ContentFile

        public long AddContentFile(long parentID, ComponentType type, string icon,
            string title, string uri, long size, string description)
        {
            if (ComponentsCache.ContainsKey(parentID))
            {
                ContentFile file = (ComponentsCache[parentID] as Lesson).FindContentFile(type, uri, size);
                if (file == null)
                {
                    if (!SaveFile(uri))
                        return -2;
                    long id = GetID();
                    string path = "cont\\" + uri.Split('\\').Last();
                    file =
                        ContentFactory.CreateLeafComponent(id, parentID, type, title, path, size, description, icon) as
                            ContentFile;
                    (ComponentsCache[parentID] as Lesson).Components.Add(file);
                    ComponentsCache[id] = file;
                    return id;
                }
                return 0;
            }
            return -1;
        }

        public void EditContentFile(long id, ComponentType type, string icon,
            string title, string uri, long size, string description)
        {
            if (ComponentsCache.ContainsKey(id))
            {
                ContentFile file = ComponentsCache[id] as ContentFile;
                file.ComponentType = type;
                file.icon = icon ?? file.icon;
                file.Title = title ?? file.Title;
                file.URI = uri ?? file.URI;
                file.Size = size;
                file.Description = description ?? file.Description;
            }
        }

        public void DeleteContentFile(long id, long parentID)
        {
            if (ComponentsCache.ContainsKey(id) && ComponentsCache.ContainsKey(parentID))
            {
                if (DeleteFile((ComponentsCache[id] as ContentFile).URI))
                {
                    (ComponentsCache[parentID] as Lesson).Components.RemoveAll(x => x.ID == id);
                    ComponentsCache.Remove(id);
                }
            }
        }

        #endregion

        #region Question

        public long AddQuestion(long parentID, string text, string answer, QuestionType type,
            ObservableCollection<string> wrongAnswers)
        {
            if (ComponentsCache.ContainsKey(parentID))
            {
                Question file = (ComponentsCache[parentID] as Quiz).FindQuestion(text, answer);
                if (file == null)
                {
                    long id = GetID();
                    if (type != QuestionType.Choice)
                        file = ContentFactory.CreateLeafComponent(id, parentID, text, answer, type) as Question;
                    else
                        //mozda u factory treba da se promeni lista
                        file =
                            ContentFactory.CreateLeafComponent(id, parentID, text, answer, type, wrongAnswers) as
                                Question;
                    (ComponentsCache[parentID] as Quiz).Components.Add(file);
                    ComponentsCache[id] = file;
                    return id;
                }
                return 0;
            }
            return -1;
        }

        public void EditQuestion(long id, string text, string answer, QuestionType type,
            ObservableCollection<string> wrongAnswers)
        {
            if (ComponentsCache.ContainsKey(id))
            {
                Question question = ComponentsCache[id] as Question;
                question.Text = text ?? question.Text;
                question.Answer = answer ?? question.Answer;
                question.Type = type;
                question.WrongAnswers = wrongAnswers;
            }
        }

        public void DeleteQuestion(long id, long parentID)
        {
            if (ComponentsCache.ContainsKey(id) && ComponentsCache.ContainsKey(parentID))
            {
                (ComponentsCache[parentID] as Quiz).Components.RemoveAll(x => x.ID == id);
                ComponentsCache.Remove(id);
            }
        }

        #endregion

        private bool SaveFile(string filePath)
        {
            try
            {
                string current = Directory.GetCurrentDirectory();
                if (!Directory.Exists(current + "\\cont"))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(current + "\\cont\\");
                    dir.Attributes = FileAttributes.Hidden | FileAttributes.Directory;
                }
                Directory.SetCurrentDirectory("cont\\");
                byte[] bytes = File.ReadAllBytes(filePath);
                byte[] encryptedBytes = EncryptFile(bytes);
                string name = filePath.Split('\\').Last();
                //File.WriteAllBytes(name, bytes);
                File.WriteAllBytes(name + ".cyp", encryptedBytes);
                Directory.SetCurrentDirectory("..\\");
                return true;
            }
            catch (Exception e)
            {
                Directory.SetCurrentDirectory("..\\");
                string current = Directory.GetCurrentDirectory();
                return false;
            }
        }

        public bool DeleteFile(string path)
        {
            try
            {
                string dir = Directory.GetCurrentDirectory();
                File.Delete(dir + "\\cont\\" +path + ".cyp");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private byte[] DecryptFile(byte[] file)
        {
            byte[] inputArray = file;
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(_encryptionKey);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return resultArray;
        }

        public string CreateNewArchive()
        {
            string archiveName = "";
            try
            {
                string dirPath = ""; //this.GetDirPath(userId, action);
                //long currentTime = (Int32)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                //string timestamp = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
                //FileInfo testExist = new FileInfo(System.IO.Path.Combine(dirPath, userId + ".zip"));
                //archiveName = ;
                //while (testExist.Exists)
                {
                    //currentTime = (Int32)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    //testExist = new FileInfo(System.IO.Path.Combine(dirPath,  + ".zip"));
                }
                //archiveName = System.IO.Path.Combine(dirPath,  + ".zip");
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

        public byte[] EncryptFile(byte[] file)
        {
            byte[] inputArray = file;
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(_encryptionKey);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return resultArray;
        }

        #endregion

        public void RecursiveDelete(Component component)
        {
            while ((component as CompositeComponent).Components.Count != 0)
            {
                Component comp = (component as CompositeComponent).Components.First();
                if (comp.GetType().Name == "ContentFile")
                {
                    DeleteContentFile(comp.ID, component.ID);
                }
                else if (comp.GetType().Name == "Question")
                    DeleteQuestion(comp.ID, component.ID);
                else
                {
                    RecursiveDelete(comp);
                    if (comp.GetType().Name == "Course")
                        DeleteCourse(comp.ID);
                    if (comp.GetType().Name == "Topic")
                        DeleteCourse(comp.ID);
                    if (comp.GetType().Name == "EducationalLevel")
                        DeleteEducationalLevel(comp.ID, component.ID);
                    if (comp.GetType().Name == "Grade")
                        DeleteGrade(comp.ID, component.ID);
                    if (comp.GetType().Name == "Week")
                        DeleteWeek(comp.ID, component.ID);
                    if (comp.GetType().Name == "Lesson")
                        DeleteLesson(comp.ID, component.ID);
                    if (comp.GetType().Name == "Quiz")
                        DeleteQuiz(comp.ID, component.ID);
                }
            }
        }
    }
}
