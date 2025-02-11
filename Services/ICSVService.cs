using System.Collections.Generic;
using School_Time_Table.Models;

namespace School_Time_Table.Services
{
    public interface ICSVService
    {
        List<Teacher> LoadTeachers();

        List<SchoolClass> LoadClasses();

        List<Schedule> LoadSchedule();

        void SaveSchedule(List<Schedule> schedules);
    }
}