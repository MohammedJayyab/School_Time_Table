using System.Collections.Generic;

namespace School_Time_Table.Services
{
    public interface IConstraintService
    {
        bool IsTeacherAvailable(string teacherId, string day, int period, 
            Dictionary<string, Dictionary<string, HashSet<int>>> teacherAvailability);

        bool IsSubjectAllowed(string classId, string subject, HashSet<string> assignedSubjects);

        bool HasReachedMaxTeachingLoad(string teacherId, string day, 
            Dictionary<string, Dictionary<string, int>> dailyLoad, 
            Dictionary<string, int> weeklyLoad);
    }
}