using SQLite;
using JournalProject.Data;
using JournalProject.Models;

namespace JournalProject.Services
{
    public class AnalyticsService
    {
        private readonly SQLiteAsyncConnection _db;

        public AnalyticsService(AppDbContext context)
        {
            _db = context.Db;
        }

        public async Task<Dictionary<string, int>> GetMoodDistributionAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var entries = await GetFilteredEntriesAsync(startDate, endDate);
            var distribution = new Dictionary<string, int>();

            foreach (var entry in entries)
            {
                if (!string.IsNullOrEmpty(entry.PrimaryMood))
                {
                    if (!distribution.ContainsKey(entry.PrimaryMood))
                        distribution[entry.PrimaryMood] = 0;
                    distribution[entry.PrimaryMood]++;
                }
            }

            return distribution.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        public async Task<Dictionary<string, int>> GetMoodCategoryDistributionAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var entries = await GetFilteredEntriesAsync(startDate, endDate);
            var positive = 0;
            var neutral = 0;
            var negative = 0;

            foreach (var entry in entries)
            {
                if (!string.IsNullOrEmpty(entry.PrimaryMood))
                {
                    var category = MoodCategories.GetMoodType(entry.PrimaryMood);
                    if (category == "Positive") positive++;
                    else if (category == "Neutral") neutral++;
                    else if (category == "Negative") negative++;
                }
            }

            return new Dictionary<string, int>
            {
                { "Positive", positive },
                { "Neutral", neutral },
                { "Negative", negative }
            };
        }

        public async Task<string?> GetMostFrequentMoodAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var distribution = await GetMoodDistributionAsync(startDate, endDate);
            return distribution.FirstOrDefault().Key;
        }

        public async Task<Dictionary<string, int>> GetMostUsedTagsAsync(int limit = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            var entries = await GetFilteredEntriesAsync(startDate, endDate);
            var tagCount = new Dictionary<string, int>();

            foreach (var entry in entries)
            {
                if (entry.Tags != null && entry.Tags.Count > 0)
                {
                    foreach (var tag in entry.Tags)
                    {
                        if (!tagCount.ContainsKey(tag))
                            tagCount[tag] = 0;
                        tagCount[tag]++;
                    }
                }
            }

            return tagCount
                .OrderByDescending(x => x.Value)
                .Take(limit)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public async Task<double> GetAverageWordCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var entries = await GetFilteredEntriesAsync(startDate, endDate);
            if (entries.Count == 0) return 0;
            return entries.Average(e => e.WordCount);
        }

        public async Task<Dictionary<DateTime, int>> GetWordCountTrendAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var entries = await GetFilteredEntriesAsync(startDate, endDate);
            var trend = new Dictionary<DateTime, int>();

            foreach (var entry in entries.OrderBy(e => e.EntryDate))
            {
                var date = entry.EntryDate.Date;
                if (!trend.ContainsKey(date))
                    trend[date] = 0;
                trend[date] = entry.WordCount;
            }

            return trend;
        }

        public async Task<List<(DateTime Date, int WordCount)>> GetWordCountTrendsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var trendDict = await GetWordCountTrendAsync(startDate, endDate);
            return trendDict.Select(kvp => (kvp.Key, kvp.Value)).ToList();
        }

        public async Task<int> GetTotalEntriesAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return (await GetFilteredEntriesAsync(startDate, endDate)).Count;
        }

        public async Task<Dictionary<string, int>> GetCategoryBreakdownAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var entries = await GetFilteredEntriesAsync(startDate, endDate);
            var breakdown = new Dictionary<string, int>();

            foreach (var entry in entries)
            {
                if (!string.IsNullOrEmpty(entry.Category))
                {
                    if (!breakdown.ContainsKey(entry.Category))
                        breakdown[entry.Category] = 0;
                    breakdown[entry.Category]++;
                }
            }

            return breakdown.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        public async Task<Dictionary<string, double>> GetCategoryDistributionPercentageAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var breakdown = await GetCategoryBreakdownAsync(startDate, endDate);
            var total = breakdown.Values.Sum();
            
            if (total == 0)
                return new Dictionary<string, double>();

            return breakdown
                .ToDictionary(x => x.Key, x => (x.Value * 100.0) / total)
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private async Task<List<JournalEntry>> GetFilteredEntriesAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var entries = await _db.Table<JournalEntry>().ToListAsync();

            if (startDate.HasValue)
                entries = entries.Where(e => e.EntryDate.Date >= startDate.Value.Date).ToList();

            if (endDate.HasValue)
                entries = entries.Where(e => e.EntryDate.Date <= endDate.Value.Date).ToList();

            return entries;
        }
    }
}
