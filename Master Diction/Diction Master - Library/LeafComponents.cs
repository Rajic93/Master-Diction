

namespace Diction_Master___Library
{
    public class LeafComponent : Component
    {
        public string Title { get; set; }
        public string URI { get; set; }
        public float Size { get; set; }
        public string Description { get; set; }
    }

    public class Question : LeafComponent
    {
        public string Text { get; set; }
        public string Answer { get; set; }
        public QuestionType Type { get; set; }
    }

    public class ContentFile : LeafComponent
    {
        public ComponentType ComponentType { get; set; }
        public string icon { get; set; }
    }

}