using System.Collections.Generic;
using School_Time_Table.Models;

namespace School_Time_Table.Services
{
    public interface IScheduleGenerator
    {
        List<Schedule> GenerateSchedule(List<SchoolClass> classes, List<Teacher> teachers);
    }
}