using System;
using System.Collections.Generic;
using System.Linq;
using School_Time_Table.Models;
using System.IO;

namespace School_Time_Table.Services
{
    public class Scheduler : IScheduler
    {
        private readonly IScheduleGenerator _scheduleGenerator;
        private readonly ISubjectService _subjectService;
        private readonly string[] _days;

        public Scheduler(IScheduleGenerator scheduleGenerator, ISubjectService subjectService)
        {
            _scheduleGenerator = scheduleGenerator;
            _subjectService = subjectService;
            
            string daysPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "days.txt");
            _days = File.ReadAllLines(daysPath);
        }

        public void ShowTeacherLoad(List<Schedule> schedules, List<Teacher> teachers)
        {
            var teacherDailyLoad = new Dictionary<string, Dictionary<string, int>>();
            var teacherWeeklyLoad = new Dictionary<string, int>();

            foreach (var teacher in teachers)
            {
                teacherDailyLoad[teacher.TeacherID] = new Dictionary<string, int>();
                foreach (var day in _days)
                {
                    teacherDailyLoad[teacher.TeacherID][day] = 0;
                }
                teacherWeeklyLoad[teacher.TeacherID] = 0;
            }

            foreach (var entry in schedules)
            {
                if (entry.TeacherID != "N/A")
                {
                    teacherDailyLoad[entry.TeacherID][entry.Day]++;
                    teacherWeeklyLoad[entry.TeacherID]++;
                }
            }

            Console.WriteLine("\n📌 Teacher Subject Load Summary");
            Console.WriteLine(new string('-', 60));

            foreach (var teacher in teachers)
            {
                Console.WriteLine($"\n👨‍🏫 Teacher: {teacher.TeacherName} (ID: {teacher.TeacherID})");
                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine("Day       | Subjects Taught");
                Console.WriteLine("-------------------------------------------------");

                foreach (var day in _days)
                {
                    Console.WriteLine($"{day,-10} | {teacherDailyLoad[teacher.TeacherID][day]} subjects");
                }

                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine($"📌 Total Weekly Subjects: {teacherWeeklyLoad[teacher.TeacherID]}");
                Console.WriteLine(new string('-', 60));
            }
        }

        public void DisplayTimetable(List<Schedule> schedules, List<SchoolClass> classes)
        {
            foreach (var schoolClass in classes)
            {
                Console.WriteLine($"\n📌 Time Table for {schoolClass.ClassName}");
                Console.WriteLine(new string('-', 50));

                Console.Write("Day       ");
                for (int period = 1; period <= 5; period++)
                {
                    Console.Write($"| P{period,-12} ");
                }
                Console.WriteLine("\n" + new string('-', 75));

                // Track weekly subject counts for this class
                var weeklySubjectCounts = schedules
                    .Where(s => s.ClassID == schoolClass.ClassID)
                    .GroupBy(s => s.Subject)
                    .ToDictionary(g => g.Key, g => g.Count());

                foreach (var day in _days)
                {
                    Console.Write($"{day,-10}");
                    var usedSubjectsForDay = new HashSet<string>(
                        schedules.Where(s => s.Day == day && s.ClassID == schoolClass.ClassID)
                                .Select(s => s.Subject)
                    );

                    for (int period = 1; period <= 5; period++)
                    {
                        var schedule = schedules.FirstOrDefault(s => 
                            s.Day == day && 
                            s.ClassID == schoolClass.ClassID && 
                            s.Period == period);

                        if (schedule != null)
                        {
                            Console.Write($"| {schedule.Subject,-7} ({schedule.TeacherID})");
                        }
                        else
                        {
                            var possibleSubject = _subjectService.LoadSubjects()
                                .Where(s => s.RequiredFor.Contains(schoolClass.Category))
                                .Where(s => !usedSubjectsForDay.Contains(s.Subject))
                                .Where(s => 
                                {
                                    var currentCount = weeklySubjectCounts.GetValueOrDefault(s.Subject, 0);
                                    return currentCount < s.WeeklyHoursByCategory[schoolClass.Category];
                                })
                                .OrderBy(x => Guid.NewGuid())
                                .FirstOrDefault();

                            if (possibleSubject != null)
                            {
                                usedSubjectsForDay.Add(possibleSubject.Subject);
                                weeklySubjectCounts[possibleSubject.Subject] = 
                                    weeklySubjectCounts.GetValueOrDefault(possibleSubject.Subject, 0) + 1;
                            }
                            
                            string display = possibleSubject != null 
                                ? $"| {possibleSubject.Subject,-7} (N/A)" 
                                : "|     ---      ";
                            Console.Write(display);
                        }
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("---------------------------------------------------------------------------");
            }
        }
    }
}