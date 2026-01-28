namespace JournalProject.Services;

public class ThemeService
{
    public event Action? OnThemeChanged;

    public bool IsDark { get; private set; }

    public void ToggleTheme()
    {
        IsDark = !IsDark;
        OnThemeChanged?.Invoke();
    }

    public void SetTheme(bool isDark)
    {
        IsDark = isDark;
        OnThemeChanged?.Invoke();
    }
}
