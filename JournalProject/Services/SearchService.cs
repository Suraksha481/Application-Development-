using SQLite;
using JournalProject.Data;
using JournalProject.Models;

namespace JournalProject.Services
{
    public class SearchService
    {
        private readonly SQLiteAsyncConnection _db;

        public SearchService(AppDbContext context)
        {
            _db = context.Db;
        }

        public async Task<List<JournalEntry>> SearchEntriesAsync(string? searchText = null, DateTime? startDate = null, 
            DateTime? endDate = null, string? mood = null, List<string>? tags = null)
        {
            var query = _db.Table<JournalEntry>();

            var results = new List<JournalEntry>();

            if (startDate.HasValue || endDate.HasValue || !string.IsNullOrEmpty(mood) || (tags?.Count > 0))
            {
                results = await query.ToListAsync();

                if (startDate.HasValue)
                    results = results.Where(e => e.EntryDate.Date >= startDate.Value.Date).ToList();

                if (endDate.HasValue)
                    results = results.Where(e => e.EntryDate.Date <= endDate.Value.Date).ToList();

                if (!string.IsNullOrEmpty(mood))
                    results = results.Where(e => e.PrimaryMood == mood || e.SecondaryMood1 == mood || e.SecondaryMood2 == mood).ToList();
            }
            else
            {
                results = await query.ToListAsync();
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                var searchLower = searchText.ToLower();
                results = results.Where(e =>
                    (e.Title?.ToLower().Contains(searchLower) ?? false) ||
                    (e.Content?.ToLower().Contains(searchLower) ?? false)
                ).ToList();
            }

            return results.OrderByDescending(e => e.EntryDate).ToList();
        }

        public async Task<List<JournalEntry>> FilterByMoodAsync(string mood)
        {
            var entries = await _db.Table<JournalEntry>().ToListAsync();
            return entries.Where(e => e.PrimaryMood == mood || e.SecondaryMood1 == mood || e.SecondaryMood2 == mood)
                .OrderByDescending(e => e.EntryDate)
                .ToList();
        }

        public async Task<List<JournalEntry>> FilterByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var entries = await _db.Table<JournalEntry>().ToListAsync();
            return entries.Where(e => e.EntryDate.Date >= startDate.Date && e.EntryDate.Date <= endDate.Date)
                .OrderByDescending(e => e.EntryDate)
                .ToList();
        }

        public async Task<List<JournalEntry>> FilterByCategoryAsync(string category)
        {
            var entries = await _db.Table<JournalEntry>().ToListAsync();
            return entries.Where(e => e.Category == category)
                .OrderByDescending(e => e.EntryDate)
                .ToList();
        }
    }
}
