using System.Collections.Generic;

namespace School_Time_Table.Services
{
    public interface ISubjectService
    {
        List<SubjectRule> LoadSubjects();
    }

    public record SubjectRule(
        string Subject,
        int Priority,
        Dictionary<string, int> WeeklyHoursByCategory,
        int MaxPerDay,
        HashSet<string> RequiredFor
    );
}