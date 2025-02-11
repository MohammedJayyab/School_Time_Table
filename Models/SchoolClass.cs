namespace School_Time_Table.Models
{
    public class SchoolClass
    {
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        public string Category { get; set; }  // General, Science, Literal

        public SchoolClass(string id, string name, string category)
        {
            ClassID = id;
            ClassName = name;
            Category = category;
        }

        public override string ToString()
        {
            return $"{ClassName} ({Category})";
        }
    }
}