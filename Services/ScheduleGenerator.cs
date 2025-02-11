// File: Services/ScheduleGenerator.cs

using School_Time_Table.Models;

namespace School_Time_Table.Services
{
    public class ScheduleGenerator : IScheduleGenerator
    {
        private readonly ISubjectService _subjectService;
        private readonly IConstraintService _constraintService;
        private readonly Random _random = new Random();
        private const int MaxPeriodsPerDay = 5;
     
        private readonly List<SubjectRule> _allSubjects;
        private readonly string[] _days;

        public ScheduleGenerator(ISubjectService subjectService, IConstraintService constraintService)
        {
            _subjectService = subjectService;
            _constraintService = constraintService;
            _allSubjects = _subjectService.LoadSubjects();
            
            string projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!
                                .Parent!.Parent!.Parent!.FullName;
            string daysPath = Path.Combine(projectDir, "data", "days.txt");
            _days = File.ReadAllLines(daysPath);
        }

        private bool IsTeacherAvailableForPeriod(string teacherId, string day, int period,
            List<Schedule> existingSchedules)
        {
            // Check if teacher is already teaching in this period
            return !existingSchedules.Any(s => 
                s.Day == day && 
                s.Period == period && 
                s.TeacherID == teacherId);
        }

        private bool IsValidScheduleForTeacher(string teacherId, string day, int period,
            Dictionary<string, Dictionary<string, int>> teacherDailyLoad,
            Dictionary<string, int> teacherWeeklyLoad,
            List<Schedule> existingSchedules)
        {
            var teacherAvailability = new Dictionary<string, Dictionary<string, HashSet<int>>>();
            foreach (var schedule in existingSchedules.Where(s => s.TeacherID == teacherId))
            {
                if (!teacherAvailability.ContainsKey(teacherId))
                    teacherAvailability[teacherId] = new Dictionary<string, HashSet<int>>();
                if (!teacherAvailability[teacherId].ContainsKey(schedule.Day))
                    teacherAvailability[teacherId][schedule.Day] = new HashSet<int>();
                teacherAvailability[teacherId][schedule.Day].Add(schedule.Period);
            }

            return _constraintService.IsTeacherAvailable(teacherId, day, period, teacherAvailability) &&
                   !_constraintService.HasReachedMaxTeachingLoad(teacherId, day, teacherDailyLoad, teacherWeeklyLoad);
        }

        private bool IsValidScheduleForSubject(SubjectRule subject, string day, SchoolClass schoolClass,
            List<Schedule> existingSchedules)
        {
            // Check daily limit
            var dailyCount = existingSchedules.Count(s => 
                s.ClassID == schoolClass.ClassID && 
                s.Day == day && 
                s.Subject == subject.Subject);
            
            if (dailyCount >= subject.MaxPerDay)
                return false;

            // Check weekly limit based on class category
            var weeklyCount = existingSchedules.Count(s => 
                s.ClassID == schoolClass.ClassID && 
                s.Subject == subject.Subject);

            var maxWeeklyHours = subject.WeeklyHoursByCategory[schoolClass.Category];
            if (weeklyCount >= maxWeeklyHours)
                return false;

            // Check if subject is required for this class category
            return subject.RequiredFor.Contains(schoolClass.Category);
        }

        private List<SubjectRule> ShuffleSubjectsForDay(
            List<SubjectRule> subjects, 
            string day, 
            SchoolClass schoolClass,
            List<Schedule> existingSchedules)
        {
            // Get subjects that haven't reached their weekly limit
            var validSubjects = subjects
                .Where(s => IsValidScheduleForSubject(s, day, schoolClass, existingSchedules))
                .OrderByDescending(s => s.Priority)
                .ThenBy(x => _random.Next())
                .ToList();

            return validSubjects;
        }

        private double CalculateSubjectScore(
            SubjectRule subject,
            string day,
            SchoolClass schoolClass,
            Dictionary<string, List<(string Day, int Period)>> weeklyPatterns)
        {
            double score = subject.Priority;  // Base score from priority

            if (!weeklyPatterns.ContainsKey(subject.Subject))
                return score + 1;  // Boost subjects not yet scheduled

            var subjectPatterns = weeklyPatterns[subject.Subject];

            // Penalize if subject was in same period recently
            var recentPeriodUses = subjectPatterns
                .GroupBy(x => x.Period)
                .Select(g => g.Count())
                .DefaultIfEmpty(0)
                .Max();
            score -= (recentPeriodUses * 0.5);

            // Penalize consecutive days
            var consecutiveDays = CountConsecutiveDays(subjectPatterns.Select(p => p.Day).ToList());
            score -= (consecutiveDays * 0.3);

            // Add small random factor
            score += _random.NextDouble() * 0.2;

            return score;
        }

        private int CountConsecutiveDays(List<string> days)
        {
            var dayIndices = days.Select(d => Array.IndexOf(_days, d)).OrderBy(i => i).ToList();
            
            int maxConsecutive = 1;
            int currentConsecutive = 1;
            
            for (int i = 1; i < dayIndices.Count; i++)
            {
                if (dayIndices[i] == dayIndices[i - 1] + 1)
                    currentConsecutive++;
                else
                    currentConsecutive = 1;
            
                maxConsecutive = Math.Max(maxConsecutive, currentConsecutive);
            }
            
            return maxConsecutive;
        }

        public List<Schedule> GenerateSchedule(List<SchoolClass> classes, List<Teacher> teachers)
        {
            var schedules = new List<Schedule>();
            var teacherDailyLoad = teachers.ToDictionary(
                t => t.TeacherID,
                t => _days.ToDictionary(day => day, day => 0)
            );
            var teacherWeeklyLoad = teachers.ToDictionary(t => t.TeacherID, t => 0);

            foreach (var schoolClass in classes.OrderBy(c => c.ClassID))
            {
                foreach (var day in _days)
                {
                    var validSubjectsForDay = ShuffleSubjectsForDay(_allSubjects, day, schoolClass, schedules);

                    for (int period = 1; period <= MaxPeriodsPerDay; period++)
                    {
                        var validSubjects = validSubjectsForDay
                            .Where(s => IsSubjectValidForClass(s.Subject, schoolClass))
                            .Where(s => IsValidScheduleForSubject(s, day, schoolClass, schedules))
                            .ToList();

                        foreach (var subject in validSubjects)
                        {
                            var availableTeachers = teachers
                                .Where(t => t.Subjects.Contains(subject.Subject))
                                .Where(t => IsValidScheduleForTeacher(
                                    t.TeacherID, 
                                    day, 
                                    period, 
                                    teacherDailyLoad, 
                                    teacherWeeklyLoad,
                                    schedules))
                                .OrderBy(t => teacherWeeklyLoad[t.TeacherID])
                                .ThenBy(t => teacherDailyLoad[t.TeacherID][day])
                                .ThenBy(x => _random.Next())  // Add randomization to teacher selection
                                .ToList();

                            if (availableTeachers.Any())
                            {
                                var teacher = availableTeachers.First();
                                teacherDailyLoad[teacher.TeacherID][day]++;
                                teacherWeeklyLoad[teacher.TeacherID]++;

                                schedules.Add(new Schedule(day, schoolClass.ClassID, period,
                                    subject.Subject, teacher.TeacherID));
                                break;
                            }
                        }
                    }
                }
            }

            return schedules.OrderBy(s => s.ClassID)
                           .ThenBy(s => s.Day)
                           .ThenBy(s => s.Period)
                           .ToList();
        }

        private bool IsSubjectValidForClass(string subject, SchoolClass schoolClass)
        {
            var subjectRule = _allSubjects.FirstOrDefault(s => s.Subject == subject);
            if (subjectRule == null) return false;
            return subjectRule.RequiredFor.Contains(schoolClass.Category);
        }
    }
}