window.theme = {
    set: function (isDark) {
        document.body.classList.toggle("dark-theme", isDark);
        document.documentElement.classList.toggle("dark-theme", isDark);
        localStorage.setItem("journalAppTheme", isDark ? "dark" : "light");
    },
    get: function () {
        return localStorage.getItem("journalAppTheme");
    },
    init: function () {
        const saved = localStorage.getItem("journalAppTheme");
        if (saved === "dark") {
            document.body.classList.add("dark-theme");
            document.documentElement.classList.add("dark-theme");
        }
    }
};

// Initialize theme on page load
document.addEventListener("DOMContentLoaded", function() {
    window.theme.init();
});
