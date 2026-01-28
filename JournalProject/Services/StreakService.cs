using SQLite;
using JournalProject.Data;
using JournalProject.Models;

namespace JournalProject.Services
{
    public class StreakService
    {
        private readonly SQLiteAsyncConnection _db;
        private readonly JournalService _journalService;

        public StreakService(AppDbContext context, JournalService journalService)
        {
            _db = context.Db;
            _journalService = journalService;
        }

        public async Task<Streak> GetStreakAsync()
        {
            var streak = await _db.Table<Streak>().FirstOrDefaultAsync();
            return streak ?? new Streak { Id = 1 };
        }

        public async Task UpdateStreakAsync()
        {
            var entries = await _journalService.GetAllAsync();
            if (entries.Count == 0)
            {
                var emptyStreak = new Streak { Id = 1, CurrentStreak = 0, LongestStreak = 0 };
                await _db.InsertOrReplaceAsync(emptyStreak);
                return;
            }

            var sortedEntries = entries.OrderByDescending(e => e.EntryDate).ToList();
            var today = DateTime.Now.Date;
            var lastEntryDate = sortedEntries.First().EntryDate.Date;

            int currentStreak = CalculateCurrentStreak(sortedEntries, today);
            int longestStreak = CalculateLongestStreak(sortedEntries);

            var streak = await GetStreakAsync();
            streak.CurrentStreak = currentStreak;
            streak.LongestStreak = longestStreak;
            streak.LastEntryDate = lastEntryDate;
            streak.TotalEntries = entries.Count;

            await _db.InsertOrReplaceAsync(streak);
        }

        private int CalculateCurrentStreak(List<JournalEntry> sortedEntries, DateTime today)
        {
            int streak = 0;
            var currentDate = today;

            foreach (var entry in sortedEntries)
            {
                if (entry.EntryDate.Date == currentDate)
                {
                    streak++;
                    currentDate = currentDate.AddDays(-1);
                }
                else if (entry.EntryDate.Date < currentDate)
                {
                    break;
                }
            }

            return streak;
        }

        private int CalculateLongestStreak(List<JournalEntry> sortedEntries)
        {
            if (sortedEntries.Count == 0) return 0;

            int longest = 1;
            int current = 1;
            var previousDate = sortedEntries.First().EntryDate.Date;

            for (int i = 1; i < sortedEntries.Count; i++)
            {
                var currentDate = sortedEntries[i].EntryDate.Date;
                if (previousDate.AddDays(-1) == currentDate)
                {
                    current++;
                    longest = Math.Max(longest, current);
                }
                else
                {
                    current = 1;
                }
                previousDate = currentDate;
            }

            return longest;
        }

        public async Task<List<DateTime>> GetMissedDaysAsync(DateTime startDate, DateTime endDate)
        {
            var entries = await _journalService.GetAllAsync();
            var entryDates = entries.Where(e => e.EntryDate.Date >= startDate && e.EntryDate.Date <= endDate)
                .Select(e => e.EntryDate.Date)
                .ToHashSet();

            var missedDays = new List<DateTime>();
            var current = startDate;
            while (current <= endDate)
            {
                if (!entryDates.Contains(current))
                {
                    missedDays.Add(current);
                }
                current = current.AddDays(1);
            }

            return missedDays;
        }
    }
}
