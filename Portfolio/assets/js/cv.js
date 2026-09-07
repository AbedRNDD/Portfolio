const navbar = document.querySelector('.topnav');
const backToTop = document.querySelector('.back-to-top');
const menuButton = document.querySelector('.icon');
const navLinks = document.querySelector('.nav-links');
let lastScroll = 0;

window.addEventListener('scroll', () => {
    const currentScroll = window.scrollY;

    if (navbar) {
        if (currentScroll > lastScroll && currentScroll > 100) {
            navbar.classList.add('nav-hidden');
        } else {
            navbar.classList.remove('nav-hidden');
        }
    }

    if (backToTop) {
        backToTop.style.display = currentScroll > 300 ? 'block' : 'none';
    }

    lastScroll = Math.max(currentScroll, 0);
});

if (menuButton && navLinks) {
    menuButton.addEventListener('click', () => {
        const isOpen = navLinks.classList.toggle('responsive');
        menuButton.setAttribute('aria-expanded', String(isOpen));
    });

    navLinks.querySelectorAll('a').forEach(link => {
        link.addEventListener('click', () => {
            if (navLinks.classList.contains('responsive')) {
                navLinks.classList.remove('responsive');
                menuButton.setAttribute('aria-expanded', 'false');
            }
        });
    });
}

if (backToTop) {
    backToTop.addEventListener('click', () => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });
}
