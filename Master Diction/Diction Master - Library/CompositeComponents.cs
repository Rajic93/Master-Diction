using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
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
    }

    public class EducationalLevel : CompositeComponent
    {
        public EducationalLevelType Level { get; set; }
    }

    public class Grade : CompositeComponent
    {
        public int Num { get; set; }
    }

    public class Week : CompositeComponent
    {
        public int Num { get; set; }
        public int Term { get; set; }
    }

    public class Lesson : CompositeComponent
    {
        public int Num { get; set; }
        public string Title { get; set; }
    }
}
