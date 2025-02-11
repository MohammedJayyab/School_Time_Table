namespace School_Time_Table.Models
{
    public class Subject
    {
        public string SubjectName { get; set; }

        public Subject(string name)
        {
            SubjectName = name;
        }

        public override string ToString()
        {
            return SubjectName;
        }
    }
}