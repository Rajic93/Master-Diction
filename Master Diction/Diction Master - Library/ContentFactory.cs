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
        public static Component CreateLeafComponent(ComponentType type, string title, string uri, string desc)
        {
            switch (type)
            {
                case ComponentType.Audio:
                    return new Audio
                    {
                        Description = desc,
                        Title = title,
                        URI = uri
                    };
                case ComponentType.Video:
                    return new Video
                    {
                        Description = desc,
                        Title = title,
                        URI = uri
                    };
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

        public static Component CreateCompositeComponent(ComponentType type, int num)
        {
            return type == ComponentType.Grade ? new Grade { Num = num } : null;
        }

        public static Component CreateCompositeComponent(ComponentType type, EducationalLevelType level)
        {
            return type == ComponentType.EducationalLevel ? new EducationalLevel { Level = level} : null;
        }

        public static Component CreateCompositeComponent(ComponentType type, string name)
        {
            return type == ComponentType.Course ? new Course { Name = name} : null;
        }
    }
}
