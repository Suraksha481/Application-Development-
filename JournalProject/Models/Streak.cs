using SQLite;

namespace JournalProject.Models
{
    public class Streak
    {
        [PrimaryKey]
        public int Id { get; set; } = 1;

        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime LastEntryDate { get; set; }
        public int TotalEntries { get; set; }

        [Ignore]
        public List<DateTime> MissedDays { get; set; } = new();
    }
}
