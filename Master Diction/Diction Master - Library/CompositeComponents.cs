using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Diction_Master___Library
{
    [XmlInclude(typeof(Course))]
    [XmlInclude(typeof(EducationalLevel))]
    [XmlInclude(typeof(Grade))]
    [XmlInclude(typeof(Week))]
    [XmlInclude(typeof(Lesson))]
    public abstract class CompositeComponent : Component
    {
        public List<Component> Components { get; set; }

        public CompositeComponent()
        {
            Components = new List<Component>();
        }

        public void Add(Component component)
        {
            Components.Add(component);
        }

        public void Delete(Component component)
        {
            if (Components.Exists(x => x.ID == component.ID))
            {
                Components.Remove(component);
            }
            else
            {
                throw new Exception("Component does not exist!");
            }
        }

        public void Update(Component oldComponent, Component newComponent)
        {
            if (Components.Exists(x => x.ID == oldComponent.ID))
            {
                //check if it needs to be deleted first
                Components.Insert(Components.IndexOf(oldComponent), newComponent);
            }
            else
            {
                throw new Exception("Component does not exist!");
            }
        }
        }

    public class Course : CompositeComponent
    {
        public string Name { get; set; }
        public string Icon { get; set; }

        public EducationalLevel FindEducationLevel(EducationalLevelType type)
        {
            if (Components.Exists(x => ((EducationalLevel)x).Level == type))
            {
                return Components.Find(x => ((EducationalLevel) x).Level == type) as EducationalLevel;
            }
            return null;
        }
    }

    public class EducationalLevel : CompositeComponent
    {
        public string Icon { get; set; }
        public EducationalLevelType Level { get; set; }

        public Grade FindGrade(GradeType type)
        {
            if (Components.Exists(x => ((Grade)x).GradeNum == type))
            {
                return Components.Find(x => ((Grade)x).GradeNum == type) as Grade;
            }
            return null;
        }
    }

    public class Grade : CompositeComponent
    {
        public string Icon { get; set; }
        public GradeType GradeNum { get; set; }

        public Week FindWeek(string title, int num, int term)
        {
            Week week = Components.Find(x => (x as Week).Title == title && ((x as Week).Num == num && (x as Week).Term == term)) as Week;
            return week;
        }
    }

    public class Week : CompositeComponent
    {
        public string Title { get; set; }
        public int Num { get; set; }
        public int Term { get; set; }

        public Lesson FindLesson(string title, int num)
        {
            return Components.Find(x => (x as Lesson).Title == title
            || (x as Lesson).Num == num) as Lesson;
        }
    }

    public class Lesson : CompositeComponent
    {
        public string Title { get; set; }
        public int Num { get; set; }

        public Quiz FindQuiz(string title)
        {
            return Components.Find(x => x.GetType().Name == "Quiz"
            && (x as Quiz).Title == title) as Quiz;
        }

        public ContentFile FindContentFile(ComponentType type, string uri, float size)
        {
            return Components.Find(x => x.GetType().Name == "ContentFile" 
            && ((x as ContentFile).ComponentType == type
            && (x as ContentFile).URI == uri && (x as ContentFile).Size == size)) 
            as ContentFile;
        }
    }

    public class Quiz : CompositeComponent
    {
        public string Title { get; set; }

        public Question FindQuestion(string text, string answer)
        {
            return Components.Find(x => (x as Question).Text == text
            && (x as Question).Answer == answer) as Question;
        }
    }
}
