const themeToggle = document.querySelector('#theme-toggle');
const themeMenu = document.querySelector('#theme-menu');
const themeIcon = document.querySelector('#theme-icon');
const themeOptions = document.querySelectorAll('[data-theme]');
const sunStatus = document.querySelector('#sun-status');

const sunApiUrl = 'https://api.open-meteo.com/v1/forecast?latitude=52.0705&longitude=4.3007&daily=sunrise,sunset&timezone=Europe%2FAmsterdam&forecast_days=1';

let selectedTheme = localStorage.getItem('portfolio-theme') || 'automatic';
let sunriseTime = null;
let sunsetTime = null;

function formatTime(dateTime) {
    return dateTime.split('T')[1].slice(0, 5);
}

function showThemeMenu(show) {
    if (!themeMenu || !themeToggle) {
        return;
    }

    themeMenu.classList.toggle('show', show);
    themeToggle.setAttribute('aria-expanded', String(show));
}

function updateThemeButtons() {
    themeOptions.forEach(button => {
        const isActive = button.dataset.theme === selectedTheme;
        button.classList.toggle('active', isActive);
        button.setAttribute('aria-pressed', String(isActive));
    });
}

function applyTheme(isDark) {
    document.body.classList.toggle('dark-mode', isDark);

    if (themeIcon) {
        themeIcon.className = isDark ? 'fa-solid fa-moon' : 'fa-solid fa-sun';
    }
}

function applyAutomaticTheme() {
    if (!sunriseTime || !sunsetTime) {
        return;
    }

    const now = new Date();
    const currentMinutes = now.getHours() * 60 + now.getMinutes();
    const [sunriseHour, sunriseMinute] = formatTime(sunriseTime).split(':').map(Number);
    const [sunsetHour, sunsetMinute] = formatTime(sunsetTime).split(':').map(Number);
    const sunriseMinutes = sunriseHour * 60 + sunriseMinute;
    const sunsetMinutes = sunsetHour * 60 + sunsetMinute;
    const isNight = currentMinutes < sunriseMinutes || currentMinutes >= sunsetMinutes;

    applyTheme(isNight);
}

function applySelectedTheme() {
    if (selectedTheme === 'dark') {
        applyTheme(true);
        return;
    }

    if (selectedTheme === 'light') {
        applyTheme(false);
        return;
    }

    applyAutomaticTheme();
}

function selectTheme(theme) {
    selectedTheme = theme;
    localStorage.setItem('portfolio-theme', theme);
    updateThemeButtons();
    applySelectedTheme();
}

function showSunTimes(sunrise, sunset) {
    if (sunStatus) {
        sunStatus.textContent = `Zonsopkomst ${formatTime(sunrise)} · Zonsondergang ${formatTime(sunset)}`;
    }
}

function showSunError() {
    if (sunStatus) {
        sunStatus.textContent = 'Zondata kon niet worden geladen.';
    }

    if (selectedTheme === 'automatic') {
        applyTheme(false);
    }
}

async function loadSunData() {
    try {
        const response = await fetch(sunApiUrl);

        if (!response.ok) {
            throw new Error('API-response was niet geldig.');
        }

        const data = await response.json();
        sunriseTime = data.daily.sunrise[0];
        sunsetTime = data.daily.sunset[0];

        showSunTimes(sunriseTime, sunsetTime);
        applySelectedTheme();
    } catch (error) {
        showSunError();
    }
}

if (themeToggle) {
    themeToggle.addEventListener('click', () => {
        showThemeMenu(!themeMenu.classList.contains('show'));
    });
}

themeOptions.forEach(button => {
    button.addEventListener('click', () => {
        selectTheme(button.dataset.theme);
    });
});

document.addEventListener('click', event => {
    if (themeMenu && themeToggle && !themeMenu.contains(event.target) && !themeToggle.contains(event.target)) {
        showThemeMenu(false);
    }
});

document.addEventListener('keydown', event => {
    if (event.key === 'Escape') {
        showThemeMenu(false);
    }
});

updateThemeButtons();

if (selectedTheme === 'dark') {
    applyTheme(true);
} else if (selectedTheme === 'light') {
    applyTheme(false);
}

loadSunData();

setInterval(() => {
    if (selectedTheme === 'automatic') {
        applyAutomaticTheme();
    }
}, 60000);
