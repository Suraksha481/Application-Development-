using SQLite;

namespace JournalProject.Models
{
    public class Tag
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string? Name { get; set; }

        public string? Category { get; set; } // e.g., "Work", "Health", "Travel"

        public int UsageCount { get; set; }

        public static readonly List<string> PredefinedTags = new()
        {
            "Work", "Career", "Studies", "Family", "Friends", "Relationships",
            "Health", "Fitness", "Personal Growth", "Self-care", "Hobbies", "Travel",
            "Nature", "Finance", "Spirituality", "Birthday", "Holiday", "Vacation",
            "Celebration", "Exercise", "Reading", "Writing", "Cooking", "Meditation",
            "Yoga", "Music", "Shopping", "Parenting", "Projects", "Planning", "Reflection"
        };
    }
}
