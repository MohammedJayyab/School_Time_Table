using System.Collections.Generic;

namespace School_Time_Table.Models
{
    public class Teacher
    {
        public string TeacherID { get; }
        public string TeacherName { get; }
        public HashSet<string> Subjects { get; } = new HashSet<string>();

        public Teacher(string id, string name)
        {
            TeacherID = id;
            TeacherName = name;
        }

        // ✅ AddSubject method to allow adding subjects
        public void AddSubject(string subject)
        {
            Subjects.Add(subject);
        }

        public override string ToString()
        {
            return $"{TeacherName} ({string.Join(", ", Subjects)})";
        }
    }
}