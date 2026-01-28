namespace JournalProject.Models
{
    public static class MoodCategories
    {
        public static readonly List<string> Positive = new()
        {
            "Happy",
            "Excited",
            "Relaxed",
            "Grateful",
            "Confident"
        };

        public static readonly List<string> Neutral = new()
        {
            "Calm",
            "Thoughtful",
            "Curious",
            "Nostalgic",
            "Bored"
        };

        public static readonly List<string> Negative = new()
        {
            "Sad",
            "Angry",
            "Stressed",
            "Lonely",
            "Anxious"
        };

        public static string GetMoodType(string mood)
        {
            if (Positive.Contains(mood)) return "Positive";
            if (Neutral.Contains(mood)) return "Neutral";
            if (Negative.Contains(mood)) return "Negative";
            return "Unknown";
        }

        public static List<string> GetAllMoods()
        {
            var all = new List<string>();
            all.AddRange(Positive);
            all.AddRange(Neutral);
            all.AddRange(Negative);
            return all;
        }

        public static List<string> GetMoodsByCategory(string input)
        {
            // If input is a category name (Positive, Neutral, Negative), return moods from that category
            if (input == "Positive") return new List<string>(Positive);
            if (input == "Neutral") return new List<string>(Neutral);
            if (input == "Negative") return new List<string>(Negative);
            
            // If input is a mood name, get its category and return moods from that category
            var category = GetMoodType(input);
            return category switch
            {
                "Positive" => new List<string>(Positive),
                "Neutral" => new List<string>(Neutral),
                "Negative" => new List<string>(Negative),
                _ => GetAllMoods()
            };
        }
    }
}
