using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using School_Time_Table.Models;

namespace School_Time_Table.Services
{
    public class CSVService : ICSVService
    {
        private readonly string dataPath;

        public CSVService()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            dataPath = Path.Combine(baseDir, "data");
            Directory.CreateDirectory(dataPath); // Creates if doesn't exist
        }

        public List<Teacher> LoadTeachers()
        {
            var teachers = new Dictionary<string, Teacher>();
            string filePath = Path.Combine(dataPath, "subjects_teachers.csv");

            if (!File.Exists(filePath)) return teachers.Values.ToList();

            foreach (var line in File.ReadLines(filePath).Skip(1))
            {
                var data = line.Split(',');
                if (!teachers.ContainsKey(data[0]))
                    teachers[data[0]] = new Teacher(data[0], data[1]);

                teachers[data[0]].AddSubject(data[2]);
            }
            return teachers.Values.ToList();
        }

        public List<SchoolClass> LoadClasses()
        {
            string filePath = Path.Combine(dataPath, "classes.csv");
            return File.Exists(filePath)
                ? File.ReadLines(filePath).Skip(1)
                    .Select(line => line.Split(','))
                    .Select(data => new SchoolClass(data[0], data[1], data[2]))
                    .ToList()
                : new List<SchoolClass>();
        }

        public List<Schedule> LoadSchedule()
        {
            string filePath = Path.Combine(dataPath, "timetable.csv");
            return File.Exists(filePath)
                ? File.ReadLines(filePath).Skip(1)
                    .Select(line => line.Split(','))
                    .Select(data => new Schedule(data[0], data[1], int.Parse(data[2]), data[3], data[4]))
                    .ToList()
                : new List<Schedule>();
        }

        public void SaveSchedule(List<Schedule> schedules)
        {
            string filePath = Path.Combine(dataPath, "timetable.csv");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            
            File.WriteAllLines(filePath,
                new[] { "Day,ClassID,Period,Subject,TeacherID" }
                .Concat(schedules.Select(s => $"{s.Day},{s.ClassID},{s.Period},{s.Subject},{s.TeacherID}")));
        }
    }
}