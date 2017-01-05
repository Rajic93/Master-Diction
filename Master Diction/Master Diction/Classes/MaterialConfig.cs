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

    public class MaterialConfig
    {
        #region Fields
        private Term[] nurseryLevel1 = new Term[3];
        private Term[] nurseryLevel2 = new Term[3];
        private Term[] primaryGrade1 = new Term[3];
        private Term[] primaryGrade2 = new Term[3];
        private Term[] primaryGrade3 = new Term[3];
        private Term[] primaryGrade4 = new Term[3];
        private Term[] primaryGrade5 = new Term[3];
        private Term[] primaryGrade6 = new Term[3];
        private Term[] secondaryJuniorGrade1 = new Term[3];
        private Term[] secondaryJuniorGrade2 = new Term[3];
        private Term[] secondaryJuniorGrade3 = new Term[3];
        private Term[] secondarySeniorGrade1 = new Term[3];
        private Term[] secondarySeniorGrade2 = new Term[3];
        private Term[] secondarySeniorGrade3 = new Term[3];


        #endregion

        #region Props

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

        #endregion

        public MaterialConfig()
        {
            #region Props initialization

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

            #endregion
        }
    }

    public class Term
    {
        #region Fields

        private Week[] weeks = new Week[5];

        #endregion

        #region Props

        public List<Week> Weeks { get; set; }
        public int TermNum { get; set; }

        #endregion

        public Term()
        {
            #region Props initialization

            Weeks = new List<Week>();

            #endregion
        }
    }

    public class Week
    {
        #region Fields

        private Lesson[] lessons = new Lesson[5];

        #endregion

        #region Props

        public int WeekNum { get; set; }
        public List<Lesson> Lessons { get; set; }

        #endregion

        public Week()
        {
            #region Props initialization

            Lessons = new List<Lesson>();

            #endregion
        }
    }

    public class Lesson
    {

        public string Name { get; set; }
        public ResourceLocation ResourceLocation { get; set; }

        public Lesson()
        {
        }

    }

}
