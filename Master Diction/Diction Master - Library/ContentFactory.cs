using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
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
                    return new ContentFile() {ComponentType = ComponentType.Audio};
                case ComponentType.Video:
                    return new ContentFile() {ComponentType = ComponentType.Video};
                case ComponentType.Document:
                    return new ContentFile() {ComponentType = ComponentType.Document};
                default:
                    return null;
            }   
        }

        public static Component CreateLeafComponent(int id, int parentID,
            string text, string answer, QuestionType type)
        {
            return new Question()
            {
                ID = id,
                ParentID = parentID,
                Text = text,
                Answer = answer
            };
        }
        /// <summary>
        /// Creates new leaf component with passed type.
        /// </summary>
        /// <param name="id">Id of component.</param>
        /// <param name="parentID">Id of parent component.</param>
        /// <param name="type">Type of leaf component. Can be Audio, Video or Document.</param>
        /// <param name="title">Title of file.</param>
        /// <param name="uri">Path to file.</param>
        /// <param name="size">Size of file.</param>
        /// <param name="desc">Description of file.</param>
        /// <param name="icon">Icon path.</param>
        /// <returns>New Audio, Video or Document component.</returns>
        public static Component CreateLeafComponent(int id, int parentID, ComponentType type,
            string title, string uri, float size, string desc, string icon)
        {
            if (type == ComponentType.Audio)
            {
                return new ContentFile()
                {
                    ID = id,
                    ParentID = parentID,
                    Description = desc,
                    Title = title,
                    URI = uri,
                    Size = size,
                    icon = icon,
                    ComponentType = ComponentType.Audio
                };
            }
            if (type == ComponentType.Video)
            {
                return new ContentFile()
                {
                    ID = id,
                    ParentID = parentID,
                    Description = desc,
                    Title = title,
                    URI = uri,
                    Size = size,
                    icon = icon,
                    ComponentType = ComponentType.Video
                };
            }
            return new ContentFile()
            {
                ID = id,
                ParentID = parentID,
                Description = desc,
                Title = title,
                URI = uri,
                Size = size,
                icon = icon,
                ComponentType = ComponentType.Document
            };
        }
        /// <summary>
        /// Creates new empty component based on passed type.
        /// </summary>
        /// <param name="type">Component type.</param>
        /// <returns>New empry component.</returns>
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
                case ComponentType.Quiz:
                    return new Quiz();
                default:
                    return null;
            }
        }
        /// <summary>
        /// Creates new Course object and sets all properties.
        /// </summary>
        /// <param name="id">ID of component.</param>
        /// <param name="name">Course name.</param>
        /// <param name="icon">Course icon.</param>
        /// <returns>New Course object.</returns>
        public static Component CreateCompositeComponent(int id, int parent, string name, string icon)
        {
            return new Course()
            {
                ID = id,
                ParentID = parent,
                Icon = icon,
                Name = name
            };
        }
        /// <summary>
        /// Creates new EducationalLevel object and sets all properties.
        /// </summary>
        /// <param name="id">ID of component.</param>
        /// <param name="level">EducationalLevel type.</param>
        /// <param name="icon">EducationalLevel icon.</param>
        /// <returns>New EducationalLevel object.</returns>
        public static Component CreateCompositeComponent(int id, int parent, EducationalLevelType level, string icon)
        {
            return new EducationalLevel()
            {
                ID = id,
                ParentID = parent,
                Level = level,
                Icon = icon
            };
        }
        /// <summary>
        /// Creates new Grade object and sets all properties.
        /// </summary>
        /// <param name="id">ID of component.</param>
        /// <param name="icon"></param>
        /// <param name="num">No. of Grade.</param>
        /// <returns>New Grade object.</returns>
        public static Component CreateCompositeComponent(int id, int parent, string icon, GradeType num)
        {
            return new Grade()
            {
                ID = id,
                ParentID = parent,
                Icon = icon,
                GradeNum = num
            };
        }
        /// <summary>
        /// Creates new Week object and sets all properties.
        /// </summary>
        /// <param name="id">ID of component.</param>
        /// <param name="title">Week title.</param>
        /// <param name="num">No. of Week.</param>
        /// <param name="term">No. of Term.</param>
        /// <returns>New Week object.</returns>
        public static Component CreateCompositeComponent(int id, int parent, string title, int num, int term)
        {
            return new Week()
            {
                ID = id,
                ParentID = parent,
                Title = title,
                Num = num,
                Term = term
            };
        }
        /// <summary>
        /// Creates new Lesson object and sets all properties.
        /// </summary>
        /// <param name="id">ID of component.</param>
        /// <param name="title">Lesson title.</param>
        /// <param name="num">No. of Lesson.</param>
        /// <returns>New Lesson object.</returns>
        public static Component CreateCompositeComponent(int id, int parent, string title, int num)
        {
            return new Lesson()
            {
                ID = id,
                ParentID = parent,
                Title = title,
                Num = num
            };
        }
        /// <summary>
        /// Creates new Quiz object and sets all properties.
        /// </summary>
        /// <param name="id">ID of component.</param>
        /// <param name="title">Quiz title.</param>
        /// <returns>New Quiz object.</returns>
        public static Component CreateCompositeComponent(int id, int parent, string title)
        {
            return new Quiz()
            {
                ID = id,
                ParentID = parent,
                Title = title
            };
        }
    }
}
