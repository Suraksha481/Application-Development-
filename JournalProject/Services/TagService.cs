using SQLite;
using JournalProject.Data;
using JournalProject.Models;

namespace JournalProject.Services
{
    public class TagService
    {
        private readonly SQLiteAsyncConnection _db;

        public TagService(AppDbContext context)
        {
            _db = context.Db;
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _db.Table<Tag>().OrderByDescending(t => t.UsageCount).ToListAsync();
        }

        public async Task<Tag?> GetTagByNameAsync(string name)
        {
            return await _db.Table<Tag>().FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<Tag> CreateOrGetTagAsync(string tagName)
        {
            var existing = await GetTagByNameAsync(tagName);
            if (existing != null)
            {
                existing.UsageCount++;
                await _db.UpdateAsync(existing);
                return existing;
            }

            var newTag = new Tag { Name = tagName, UsageCount = 1 };
            await _db.InsertAsync(newTag);
            return newTag;
        }

        public async Task DeleteTagAsync(int tagId)
        {
            await _db.DeleteAsync<Tag>(tagId);
        }

        public List<string> GetPredefinedTags()
        {
            return Tag.PredefinedTags;
        }

        public async Task<List<Tag>> GetMostUsedTagsAsync(int limit = 10)
        {
            return await _db.Table<Tag>()
                .OrderByDescending(t => t.UsageCount)
                .Take(limit)
                .ToListAsync();
        }
    }
}
