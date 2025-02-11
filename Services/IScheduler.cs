using System.Collections.Generic;
using School_Time_Table.Models;

namespace School_Time_Table.Services
{
    public interface IScheduler
    {
        //List<Schedule> GenerateSchedule(List<SchoolClass> classes, List<Teacher> teachers);

        void DisplayTimetable(List<Schedule> schedules, List<SchoolClass> classes);

        void ShowTeacherLoad(List<Schedule> schedules, List<Teacher> teachers); // ✅ Ensure this exists
    }
}