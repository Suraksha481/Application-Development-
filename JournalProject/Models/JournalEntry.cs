using SQLite;

namespace JournalProject.Models
{
    public class JournalEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public DateTime EntryDate { get; set; }

        public string? Title { get; set; }
        public string? Content { get; set; }

        public string? PrimaryMood { get; set; }
        public string? SecondaryMood1 { get; set; }
        public string? SecondaryMood2 { get; set; }

        public string? Category { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int WordCount { get; set; }

        public bool IsPasswordProtected { get; set; } = false;
        public bool IsEncrypted { get; set; } = false;
        public string? PasswordHash { get; set; }

        // Lock Features
        public bool IsLocked { get; set; } = false;
        public string? LockPasswordHash { get; set; }
        public string? PasswordType { get; set; } = "Strong"; // "Strong", "PIN", "Simple"
        public DateTime? LockedAt { get; set; }

        // Tags stored as JSON string for persistence
        public string? TagsJson { get; set; } = "[]";

        [Ignore]
        public List<string> Tags 
        { 
            get 
            { 
                if (string.IsNullOrEmpty(TagsJson))
                    return new();
                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<List<string>>(TagsJson) ?? new();
                }
                catch
                {
                    return new();
                }
            }
            set 
            { 
                TagsJson = System.Text.Json.JsonSerializer.Serialize(value ?? new());
            }
        }

        [Ignore]
        public List<string> SecondaryMoods { get; set; } = new();
    }
}
