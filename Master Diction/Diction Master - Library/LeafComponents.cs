 

using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Diction_Master___Library
{
    [Serializable]
    [XmlInclude(typeof(Question))] 
    [XmlInclude(typeof(ContentFile))]
    public class LeafComponent : Component
    {
        public string Title { get; set; }
        public string URI { get; set; }
        public long Size { get; set; }
        public string Description { get; set; }
    }

    [Serializable]
    public class Question : LeafComponent
    {
        public string Text { get; set; }
        public string Answer { get; set; }
        public QuestionType Type { get; set; }
        public ObservableCollection<string> WrongAnswers { get; set; }
        public ObservableCollection<string> Pieces { get; set; }
    }

    [Serializable]
    public class ContentFile : LeafComponent
    {
        public ComponentType ComponentType { get; set; }
        public string icon { get; set; }
    }

}