using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    public static class ContentFactory
    {
        public static Component CreateLeafComponent(ComponentType type, int id)
        {
            switch (type)
            {
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

        public static Component CreateLeafComponent(ComponentType type, string title, string uri, string desc)
        {
            switch (type)
            {
                case ComponentType.Audio:
                    return new LeafComponent()
                    {
                        Description = desc,
                        Title = title,
                        URI = uri,
                        ComponentType = ComponentType.Audio
                    };
                case ComponentType.Video:
                    return new LeafComponent()
                    {
                        Description = desc,
                        Title = title,
                        URI = uri,
                        ComponentType = ComponentType.Video
                    };
                case ComponentType.Document:
                    return new LeafComponent()
                    {
                        Description = desc,
                        Title = title,
                        URI = uri,
                        ComponentType = ComponentType.Document
                    };
                default:
                    return null;
            }
        }

        public static Component CreateCompositeComponent(ComponentType type)
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
                default:
                    return null;
            }
        }

        public static Component CreateCompositeComponent(ComponentType type, int num, int term)
        {
            return type == ComponentType.Week ? new Week { Num = num, Term = term } : null;
        }

        public static Component CreateCompositeComponent(ComponentType type, int num, string title)
        {
            return type == ComponentType.Lesson ? new Lesson { Num = num, Title = title } : null;
        }
        public static Component CreateCompositeComponent(ComponentType type, EducationalLevelType level)
        {
            return type == ComponentType.EducationalLevel ? new EducationalLevel() { Level = level } : null;
        }

        public static Component CreateCompositeComponent(ComponentType type, GradeType grade)
        {
            return type == ComponentType.Grade ? new Grade { GradeNum = grade } : null;
        }

        public static Component CreateCompositeComponent(ComponentType type, bool edu = true)
        {
            return type == ComponentType.EducationalLevel ? new EducationalLevel() : null;
        }

        public static Component CreateCompositeComponent(ComponentType type, string name)
        {
            return type == ComponentType.Course ? new Course { Name = name} : null;
        }

        public static Component CreateCompositeComponent(ComponentType type, int num, int term, string title)
        {
            return type == ComponentType.Week ? new Week() { Num = num, Term = term, Title = title}: null;
        }
    }
}
