using System;
using School_Time_Table.Services;

internal class Program
{
    private static void Main()
    {
        ICSVService csvService = new CSVService();
        ISubjectService subjectService = new SubjectService();
        IConstraintService constraintService = new ConstraintService();

        IScheduleGenerator scheduleGenerator = new ScheduleGenerator(subjectService, constraintService);
        IScheduler scheduler = new Scheduler(scheduleGenerator, subjectService);

        var classes = csvService.LoadClasses();
        var teachers = csvService.LoadTeachers();
        var schedules = scheduleGenerator.GenerateSchedule(classes, teachers);

        csvService.SaveSchedule(schedules);

        Console.WriteLine("\n✅ Timetable generated and saved successfully!\n");

        scheduler.DisplayTimetable(schedules, classes);

        // 📌 Show teacher subject load per day & week
        scheduler.ShowTeacherLoad(schedules, teachers);
    }
}