using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace School_Time_Table.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly string filePath;

        public SubjectService()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataDir = Path.Combine(baseDir, "data");
            Directory.CreateDirectory(dataDir); // Creates if doesn't exist
            filePath = Path.Combine(dataDir, "subjects.csv");
        }

        public List<SubjectRule> LoadSubjects()
        {
            if (!File.Exists(filePath)) 
                throw new FileNotFoundException($"Subjects file not found at: {filePath}");

            return File.ReadLines(filePath)
                .Skip(1) // Skip header
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => 
                {
                    var parts = line.Split(',');
                    if (parts.Length < 7)
                        throw new FormatException($"Invalid subject data format: {line}");
                        
                    var weeklyHours = new Dictionary<string, int>
                    {
                        ["General"] = int.Parse(parts[2].Trim()),
                        ["Science"] = int.Parse(parts[3].Trim()),
                        ["Literal"] = int.Parse(parts[4].Trim())
                    };

                    return new SubjectRule(
                        Subject: parts[0].Trim(),
                        Priority: int.Parse(parts[1].Trim()),
                        WeeklyHoursByCategory: weeklyHours,
                        MaxPerDay: int.Parse(parts[5].Trim()),
                        RequiredFor: new HashSet<string>(parts[6].Split('|').Select(x => x.Trim()))
                    );
                })
                .ToList();
        }
    }
}