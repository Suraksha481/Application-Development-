using SQLite;
using JournalProject.Data;
using JournalProject.Models;

namespace JournalProject.Services
{
    public class JournalService
    {
        private readonly SQLiteAsyncConnection _db;

        public JournalService(AppDbContext context)
        {
            _db = context.Db;
        }

        public async Task SaveAsync(JournalEntry entry)
        {
            entry.UpdatedAt = DateTime.Now;
            entry.WordCount = entry.Content?.Split(' ').Length ?? 0;

            var existing = await _db.Table<JournalEntry>()
                .FirstOrDefaultAsync(e => e.EntryDate == entry.EntryDate);

            if (existing == null)
            {
                entry.CreatedAt = DateTime.Now;
                await _db.InsertAsync(entry);
            }
            else
            {
                entry.Id = existing.Id;
                await _db.UpdateAsync(entry);
            }
        }

        public async Task DeleteAsync(DateTime date)
        {
            var entry = await _db.Table<JournalEntry>()
                .FirstOrDefaultAsync(e => e.EntryDate == date);

            if (entry != null)
                await _db.DeleteAsync(entry);
        }

        public Task<List<JournalEntry>> GetAllAsync()
            => _db.Table<JournalEntry>()
                  .OrderByDescending(e => e.EntryDate)
                  .ToListAsync();

        public void DeleteEntry(int id)
        {
            var entry = _db.Table<JournalEntry>()
                .FirstOrDefaultAsync(e => e.Id == id).Result;

            if (entry != null)
                _db.DeleteAsync(entry).Wait();
        }

        public async Task<JournalEntry?> GetEntryByIdAsync(int id)
        {
            return await _db.Table<JournalEntry>().FirstOrDefaultAsync(e => e.Id == id);
        }

        public PaginationModel<JournalEntry> GetPaginatedEntries(int pageNumber, int pageSize)
        {
            var allEntries = _db.Table<JournalEntry>()
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync().Result;

            var totalItems = allEntries.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var items = allEntries
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginationModel<JournalEntry>
            {
                Items = items,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<List<JournalEntry>> GetEntries()
        {
            return await GetAllAsync();
        }

        public async Task UpdateEntry(JournalEntry entry)
        {
            await SaveAsync(entry);
        }

        // Lock/Unlock Methods
        public async Task<bool> LockEntryAsync(int entryId, string password, string passwordType, SecurityService securityService)
        {
            var entry = await GetEntryByIdAsync(entryId);
            if (entry == null)
                throw new InvalidOperationException("Entry not found");

            var validation = securityService.ValidateLockPassword(password, passwordType);
            if (!validation.isValid)
                throw new InvalidOperationException($"Password validation failed: {string.Join(", ", validation.issues)}");

            entry.IsLocked = true;
            entry.LockPasswordHash = securityService.HashLockPassword(password);
            entry.PasswordType = passwordType;
            entry.LockedAt = DateTime.Now;

            await UpdateEntry(entry);
            return true;
        }

        public async Task<bool> UnlockEntryAsync(int entryId, string password, SecurityService securityService)
        {
            var entry = await GetEntryByIdAsync(entryId);
            if (entry == null)
                throw new InvalidOperationException("Entry not found");

            if (!entry.IsLocked)
                throw new InvalidOperationException("Entry is not locked");

            if (string.IsNullOrEmpty(entry.LockPasswordHash))
                throw new InvalidOperationException("Lock password not set");

            if (!securityService.VerifyLockPassword(password, entry.LockPasswordHash))
                return false; // Wrong password

            entry.IsLocked = false;
            entry.LockPasswordHash = null;
            entry.PasswordType = null;
            entry.LockedAt = null;

            await UpdateEntry(entry);
            return true;
        }

        public async Task<bool> IsEntryLockedAsync(int entryId)
        {
            var entry = await GetEntryByIdAsync(entryId);
            return entry?.IsLocked ?? false;
        }

        public async Task<JournalEntry?> GetEntryIfUnlockedAsync(int entryId, string? password = null, SecurityService? securityService = null)
        {
            var entry = await GetEntryByIdAsync(entryId);
            if (entry == null)
                return null;

            if (!entry.IsLocked)
                return entry;

            // If locked and no password provided, return null
            if (string.IsNullOrEmpty(password) || securityService == null)
                return null;

            // Verify password
            if (securityService.VerifyLockPassword(password, entry.LockPasswordHash ?? ""))
                return entry;

            return null;
        }
    }
}
