// Theme switching functionality
(function() {
    const THEME_STORAGE_KEY = 'journal-app-theme';

    // Apply theme CSS dynamically
    window.applyTheme = function(themeCss) {
        // Remove existing theme style if it exists
        const existingStyle = document.getElementById('theme-styles');
        if (existingStyle) {
            existingStyle.remove();
        }

        // Create and inject new theme style
        const styleElement = document.createElement('style');
        styleElement.id = 'theme-styles';
        styleElement.innerHTML = themeCss;
        document.head.appendChild(styleElement);

        // Trigger a visual refresh
        document.documentElement.style.colorScheme = themeCss.includes('#121212') ? 'dark' : 'light';
    };

    // Save theme preference to local storage
    window.saveThemePreference = function(theme) {
        try {
            localStorage.setItem(THEME_STORAGE_KEY, theme);
        } catch (e) {
            console.warn('Failed to save theme preference:', e);
        }
    };

    // Get saved theme preference
    window.getThemePreference = function() {
        try {
            return localStorage.getItem(THEME_STORAGE_KEY) || null;
        } catch (e) {
            console.warn('Failed to load theme preference:', e);
            return null;
        }
    };

    // Apply theme on page load (before Blazor starts)
    window.applyInitialTheme = function() {
        const savedTheme = window.getThemePreference();
        
        // Default light theme CSS
        const lightThemeCss = `
            :root {
                --primary-bg: #ffffff;
                --secondary-bg: #f8f9fa;
                --text-color: #000000;
                --border-color: #dee2e6;
                --accent-color: #007bff;
                --card-bg: #ffffff;
                --card-text: #000000;
                --button-bg: #007bff;
                --button-text: #ffffff;
            }
            body {
                background-color: var(--secondary-bg);
                color: var(--text-color);
            }
            .container, .container-fluid {
                background-color: var(--primary-bg);
                color: var(--text-color);
            }
            .card {
                background-color: var(--card-bg);
                border-color: var(--border-color);
                color: var(--card-text);
            }
            .btn-primary {
                background-color: var(--button-bg);
                border-color: var(--button-bg);
                color: var(--button-text);
            }
            input, textarea, select {
                background-color: var(--primary-bg);
                color: var(--text-color);
                border-color: var(--border-color);
            }
            input:focus, textarea:focus, select:focus {
                background-color: var(--primary-bg);
                color: var(--text-color);
                border-color: var(--accent-color);
            }
            .form-control {
                background-color: var(--primary-bg);
                color: var(--text-color);
                border-color: var(--border-color);
            }
            .form-control:focus {
                background-color: var(--primary-bg);
                color: var(--text-color);
                border-color: var(--accent-color);
                box-shadow: 0 0 0 0.2rem rgba(13, 110, 253, 0.25);
            }
            .alert-info {
                background-color: #d1ecf1;
                border-color: #bee5eb;
                color: #0c5460;
            }
        `;

        // Dark theme CSS
        const darkThemeCss = `
            :root {
                --primary-bg: #121212;
                --secondary-bg: #1e1e1e;
                --text-color: #e8e8e8;
                --border-color: #3a3a3a;
                --accent-color: #4a9eff;
                --card-bg: #2a2a2a;
                --card-text: #e8e8e8;
                --button-bg: #4a9eff;
                --button-text: #000000;
            }
            body {
                background-color: var(--secondary-bg);
                color: var(--text-color);
            }
            .container, .container-fluid {
                background-color: var(--primary-bg);
                color: var(--text-color);
            }
            .card {
                background-color: var(--card-bg);
                border-color: var(--border-color);
                color: var(--card-text);
            }
            .btn-primary {
                background-color: var(--button-bg);
                border-color: var(--button-bg);
                color: var(--button-text);
            }
            .btn-outline-secondary {
                color: #b0b0b0;
                border-color: #b0b0b0;
            }
            .btn-outline-secondary:hover {
                color: #4a9eff;
                border-color: #4a9eff;
                background-color: rgba(74, 158, 255, 0.1);
            }
            input, textarea, select {
                background-color: var(--card-bg);
                color: var(--text-color);
                border-color: var(--border-color);
            }
            input:focus, textarea:focus, select:focus {
                background-color: var(--card-bg);
                color: var(--text-color);
                border-color: var(--accent-color);
            }
            .form-control {
                background-color: var(--card-bg);
                color: var(--text-color);
                border-color: var(--border-color);
            }
            .form-control:focus {
                background-color: var(--card-bg);
                color: var(--text-color);
                border-color: var(--accent-color);
                box-shadow: 0 0 0 0.2rem rgba(74, 158, 255, 0.25);
            }
            .alert-info {
                background-color: #1c4f63;
                border-color: #2a6f7f;
                color: #a8d8ea;
            }
            .alert-secondary {
                background-color: #3a3a3a;
                border-color: #4a4a4a;
                color: #d1d1d1;
            }
            .alert-success {
                background-color: #1c4f3f;
                border-color: #2a7f5f;
                color: #a8e8c8;
            }
            .alert-danger {
                background-color: #4f2c2c;
                border-color: #7f3a3a;
                color: #e8a8a8;
            }
            .alert-warning {
                background-color: #4f4a2c;
                border-color: #7f7a3a;
                color: #e8e0a8;
            }
            .text-muted {
                color: #808080 !important;
            }
            .badge {
                background-color: var(--accent-color);
                color: var(--button-text);
            }
            a {
                color: #4a9eff;
            }
            a:hover {
                color: #7ab4ff;
            }
        `;

        if (savedTheme === 'Dark') {
            window.applyTheme(darkThemeCss);
        } else {
            window.applyTheme(lightThemeCss);
        }
    };

    // Apply theme on DOMContentLoaded
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', window.applyInitialTheme);
    } else {
        window.applyInitialTheme();
    }
})();
