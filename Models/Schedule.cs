using System;

namespace School_Time_Table.Models
{
    public class Schedule
    {
        public string Day { get; set; }
        public string ClassID { get; set; }
        public int Period { get; set; }
        public string Subject { get; set; }
        public string TeacherID { get; set; }

        public Schedule(string day, string classId, int period, string subject, string teacherId)
        {
            Day = day;
            ClassID = classId;
            Period = period;
            Subject = subject;
            TeacherID = teacherId;
        }

        public override string ToString()
        {
            return $"{Day} | Class: {ClassID} | Period: {Period} | Subject: {Subject} | Teacher: {TeacherID}";
        }
    }
}