using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Master_Diction.Classes
{
    class ResourcesManager
    {
        static MaterialConfig config = new MaterialConfig();

        public static MaterialConfig ConfigureResources()
        {
            //configuring Nursery level 1
            for (int i = 0; i < 3; i++)
            {
                Term term = new Term();
                term.TermNum = i + 1;
                for (int j = 0; j < 4; j++)
                {
                    Week week = new Week();
                    week.WeekNum = j + 1;
                    for (int k = 0; k < 3; k++)
                    {
                        //Lesson lesson = new Lesson();
                        //lesson.LessonNum = k + 1;
                        //for (int l = 0; l < 2; l++)
                        //{
                        //    Video video = new Video();
                        //    video.Name = "Test video";
                        //    //video.ResourceLocation = "";
                        //    lesson.videos.Add(video);
                        //}
                        //week.lessons.Add(lesson);
                    }
                    term.Weeks.Add(week);
                }
                config.NurseryLevel1.Add(term);
            }
            return config;
        }

        public static void UpdateResources()
        {
            
        }
    }
}
