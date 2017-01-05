

namespace Diction_Master___Library
{
    public abstract class LeafComponent : Component
    {
        public string Title { get; set; }
        public string URI { get; set; }
        public float Size { get; set; }
        public string Description { get; set; }
    }

    public class Audio : LeafComponent
    {
        
    }

    public class Video : LeafComponent
    {
        
    }

}