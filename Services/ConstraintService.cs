using System.Collections.Generic;

namespace School_Time_Table.Services
{
    public class ConstraintService : IConstraintService
    {
        private const int MaxDailyTeachingLoad = 5;
        private const int MaxWeeklyTeachingLoad = 35;

        public bool IsTeacherAvailable(string teacherId, string day, int period, Dictionary<string, Dictionary<string, HashSet<int>>> teacherAvailability)
        {
            return !teacherAvailability.ContainsKey(teacherId) ||
                   !teacherAvailability[teacherId].ContainsKey(day) ||
                   !teacherAvailability[teacherId][day].Contains(period);
        }

        public bool IsSubjectAllowed(string classId, string subject, HashSet<string> assignedSubjects)
        {
            return !assignedSubjects.Contains(subject);
        }

        public bool HasReachedMaxTeachingLoad(string teacherId, string day, Dictionary<string, Dictionary<string, int>> dailyLoad, Dictionary<string, int> weeklyLoad)
        {
            var currentDailyLoad = dailyLoad.GetValueOrDefault(teacherId)?.GetValueOrDefault(day, 0) ?? 0;
            var currentWeeklyLoad = weeklyLoad.GetValueOrDefault(teacherId, 0);

            return currentDailyLoad >= MaxDailyTeachingLoad || currentWeeklyLoad >= MaxWeeklyTeachingLoad;
        }
    }
}