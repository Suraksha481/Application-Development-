using System.Security.Cryptography;
using System.Text;

namespace JournalProject.Services
{
    public class SecurityService
    {
        private const string PinKey = "journal_pin";
        private const string EnabledKey = "pin_enabled";

        public async Task SetPinAsync(string pin)
        {
            if (string.IsNullOrEmpty(pin))
                throw new ArgumentException("PIN cannot be empty");

            var hash = Hash(pin);
            await SecureStorage.SetAsync(PinKey, hash);
            await SecureStorage.SetAsync(EnabledKey, "true");
        }

        public async Task<bool> VerifyPinAsync(string pin)
        {
            var stored = await SecureStorage.GetAsync(PinKey);
            if (string.IsNullOrEmpty(stored))
                return true; // No PIN set

            return stored == Hash(pin);
        }

        public async Task<bool> IsPinEnabledAsync()
        {
            var enabled = await SecureStorage.GetAsync(EnabledKey);
            return !string.IsNullOrEmpty(enabled) && bool.Parse(enabled);
        }

        public async Task DisablePinAsync()
        {
            SecureStorage.Remove(PinKey);
            SecureStorage.Remove(EnabledKey);
        }

        private string Hash(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

        public (bool isValid, List<string> issues) ValidatePasswordStrength(string password)
        {
            var issues = new List<string>();

            if (password.Length < 8)
                issues.Add("Password must be at least 8 characters");
            if (!password.Any(char.IsUpper))
                issues.Add("Password must contain an uppercase letter");
            if (!password.Any(char.IsLower))
                issues.Add("Password must contain a lowercase letter");
            if (!password.Any(char.IsDigit))
                issues.Add("Password must contain a digit");

            return (issues.Count == 0, issues);
        }

        public async Task<bool> ValidatePIN(string pin)
        {
            return await VerifyPinAsync(pin);
        }

        // Journal Entry Lock Methods
        public string HashLockPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be empty");

            return Hash(password);
        }

        public bool VerifyLockPassword(string inputPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedHash))
                return false;

            var inputHash = Hash(inputPassword);
            return inputHash == storedHash;
        }

        public (bool isValid, List<string> issues) ValidateLockPassword(string password, string passwordType)
        {
            var issues = new List<string>();

            if (string.IsNullOrEmpty(password))
            {
                issues.Add("Password cannot be empty");
                return (false, issues);
            }

            switch (passwordType)
            {
                case "Strong":
                    return ValidatePasswordStrength(password);

                case "PIN":
                    if (password.Length < 4 || password.Length > 8)
                        issues.Add("PIN must be between 4 and 8 characters");
                    if (!password.All(char.IsDigit))
                        issues.Add("PIN must contain only digits");
                    break;

                case "Simple":
                    if (password.Length < 4)
                        issues.Add("Simple password must be at least 4 characters");
                    break;
            }

            return (issues.Count == 0, issues);
        }

        public List<string> GetPasswordTypes()
        {
            return new List<string> { "Strong", "PIN", "Simple" };
        }
    }
}
