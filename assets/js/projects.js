const projects = [
    {
        title: 'Hotel Simulator',
        category: 'Java',
        description: 'Een Java-applicatie die de werking van een hotel simuleert. In het project werkte ik met gasten, kamers, medewerkers, events en softwarepatronen zoals MVC en Observer.',
        technologies: 'Java, OOP, MVC, Observer, JSON',
        image: 'img/hotel-simulator.png',
        alt: 'Interface van de Hotel Simulator',
        github: ''
    },
    {
        title: 'Lingo',
        category: 'Web',
        description: 'Een webversie van het woordspel Lingo. De speler probeert het juiste woord te raden en krijgt na iedere poging visuele feedback over de letters.',
        technologies: 'HTML, CSS, JavaScript',
        image: 'img/lingo.png',
        alt: 'Interface van het Lingo-spel',
        github: 'https://github.com/AbedRNDD/Lingo-game'
    },
    {
        title: 'Vang de Volger',
        category: 'Java',
        description: 'Een Java-spel waarin de speler zich over een speelveld beweegt terwijl een volger de speler probeert te bereiken. Ik werkte hierbij aan spellogica, pathfinding en een grafische interface.',
        technologies: 'Java, Swing, OOP, BFS, pathfinding',
        image: 'img/vang-de-volger.png',
        alt: 'Speelveld van Vang de Volger',
        github: 'https://github.com/AbedRNDD/Vang-de-volger'
    }
];

const projectGrid = document.querySelector('#projects-grid');
const filterButtons = document.querySelectorAll('[data-filter]');
const sortSelect = document.querySelector('#project-sort');

let activeFilter = 'Alles';
let activeSort = 'default';

function createProjectCard(project) {
    const card = project.github ? document.createElement('a') : document.createElement('article');
    card.className = 'project-card';

    if (project.github) {
        card.href = project.github;
        card.target = '_blank';
        card.rel = 'noopener noreferrer';
        card.setAttribute('aria-label', `${project.title} bekijken op GitHub`);
    }

    const image = document.createElement('img');
    image.src = project.image;
    image.alt = project.alt;

    const details = document.createElement('div');
    details.className = 'project-details';

    const title = document.createElement('h2');
    title.textContent = project.title;

    const description = document.createElement('p');
    description.textContent = project.description;

    const technologies = document.createElement('p');
    const label = document.createElement('strong');
    label.textContent = 'Technieken: ';
    technologies.appendChild(label);
    technologies.appendChild(document.createTextNode(project.technologies));

    details.appendChild(title);
    details.appendChild(description);
    details.appendChild(technologies);
    card.appendChild(image);
    card.appendChild(details);

    return card;
}

function getVisibleProjects() {
    let visibleProjects = projects.filter(project => {
        return activeFilter === 'Alles' || project.category === activeFilter;
    });

    if (activeSort === 'az') {
        visibleProjects = [...visibleProjects].sort((a, b) => a.title.localeCompare(b.title));
    }

    if (activeSort === 'za') {
        visibleProjects = [...visibleProjects].sort((a, b) => b.title.localeCompare(a.title));
    }

    return visibleProjects;
}

function renderProjects() {
    if (!projectGrid) {
        return;
    }

    projectGrid.textContent = '';

    getVisibleProjects().forEach(project => {
        projectGrid.appendChild(createProjectCard(project));
    });
}

function setProjectFilter(filter) {
    activeFilter = filter;

    filterButtons.forEach(button => {
        const isActive = button.dataset.filter === filter;
        button.classList.toggle('active', isActive);
        button.setAttribute('aria-pressed', String(isActive));
    });

    renderProjects();
}

filterButtons.forEach(button => {
    button.addEventListener('click', () => {
        setProjectFilter(button.dataset.filter);
    });
});

if (sortSelect) {
    sortSelect.addEventListener('change', () => {
        activeSort = sortSelect.value;
        renderProjects();
    });
}

renderProjects();
