using SQLite;
using JournalProject.Models;

namespace JournalProject.Data
{
    public class AppDbContext
    {
        private SQLiteAsyncConnection? _db;
        private bool _initStarted = false;
        private bool _initCompleted = false;

        public SQLiteAsyncConnection Db
        {
            get
            {
                if (_db == null)
                {
                    try
                    {
                        string dbPath = string.Empty;
                        try
                        {
                            dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal.db");
                        }
                        catch
                        {
                            // Fallback to a temp directory if AppDataDirectory fails
                            dbPath = Path.Combine(Path.GetTempPath(), "journal.db");
                        }

                        _db = new SQLiteAsyncConnection(dbPath);
                        if (!_initStarted)
                        {
                            InitializeDatabaseAsync().FireAndForget();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"DB Connection error: {ex.Message}");
                        // Return a dummy connection that won't crash
                        _db = new SQLiteAsyncConnection(":memory:");
                    }
                }
                return _db;
            }
        }

        private async Task InitializeDatabaseAsync()
        {
            if (_initStarted || _initCompleted) return;
            _initStarted = true;

            try
            {
                if (_db != null)
                {
                    await _db.CreateTableAsync<JournalEntry>();
                    await _db.CreateTableAsync<Tag>();
                    await _db.CreateTableAsync<Streak>();
                    _initCompleted = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database init error: {ex.Message}");
                _initStarted = false; // Allow retry
            }
        }
    }
}

