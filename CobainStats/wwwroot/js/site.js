document.addEventListener("DOMContentLoaded", function () {
    const body = document.body;
    const toggleBtn = document.getElementById("themeToggle");

    let currentTheme = localStorage.getItem("theme");

    if (!currentTheme) {
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        currentTheme = prefersDark ? "dark" : "light";
        localStorage.setItem("theme", currentTheme);
    }

    applyTheme(currentTheme);

    toggleBtn?.addEventListener("click", () => {
        const newTheme = body.classList.contains("dark-theme") ? "light" : "dark";
        applyTheme(newTheme);
        localStorage.setItem("theme", newTheme);
    });

    function applyTheme(theme) {
        body.classList.remove("light-theme", "dark-theme");
        body.classList.add(`${theme}-theme`);
    }
});
