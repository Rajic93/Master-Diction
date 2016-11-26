using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Master_Diction.Classes
{
    public enum Grades
    {
        NurseryLevel1,
        NurseryLevel2,
        PrimaryGrade1,
        PrimaryGrade2,
        PrimaryGrade3,
        PrimaryGrade4,
        PrimaryGrade5,
        PrimaryGrade6,
        SecondaryJunior1,
        SecondaryJunior2,
        SecondaryJunior3,
        SecondarySenior4,
        SecondarySenior5,
        SecondarySenior6
    }

    public class Term
    {
        public List<Week> Weeks { get; set; }
        public int TermNum { get; set; }

        public Term()
        {
            Weeks = new List<Week>();
        }
    }

    public class Week
    {
        public int WeekNum { get; set; }
        public List<Lesson> lessons { get; set; }

        public Week()
        {
            lessons = new List<Lesson>();
        }
    }

    public class Lesson
    {
        public int LessonNum { get; set; }
        public List<Video> videos { get; set; }

        public Lesson()
        {
            videos = new List<Video>();
        }

    }

    public class Video
    {
        public string Name { get; set; }
        public ResourceLocation ResourceLocation { get; set; }

    }

    public class MaterialConfig
    {
        public List<Term> NurseryLevel1 { get; set; }
        public List<Term> NurseryLevel2 { get; set; }
        public List<Term> PrimaryGrade1 { get; set; }
        public List<Term> PrimaryGrade2 { get; set; }
        public List<Term> PrimaryGrade3 { get; set; }
        public List<Term> PrimaryGrade4 { get; set; }
        public List<Term> PrimaryGrade5 { get; set; }
        public List<Term> PrimaryGrade6 { get; set; }
        public List<Term> SecondaryJuniorGrade1 { get; set; }
        public List<Term> SecondaryJuniorGrade2 { get; set; }
        public List<Term> SecondaryJuniorGrade3 { get; set; }
        public List<Term> SecondarySeniorGrade4 { get; set; }
        public List<Term> SecondarySeniorGrade5 { get; set; }
        public List<Term> SecondarySeniorGrade6 { get; set; }

        public MaterialConfig()
        {
            NurseryLevel1 = new List<Term>();
            NurseryLevel2 = new List<Term>();
            PrimaryGrade1 = new List<Term>();
            PrimaryGrade2 = new List<Term>();
            PrimaryGrade3 = new List<Term>();
            PrimaryGrade4 = new List<Term>();
            PrimaryGrade5 = new List<Term>();
            PrimaryGrade6 = new List<Term>();
            SecondaryJuniorGrade1 = new List<Term>();
            SecondaryJuniorGrade2 = new List<Term>();
            SecondaryJuniorGrade3 = new List<Term>();
            SecondarySeniorGrade4 = new List<Term>();
            SecondarySeniorGrade5 = new List<Term>();
            SecondarySeniorGrade6 = new List<Term>();
        }
    }

}
