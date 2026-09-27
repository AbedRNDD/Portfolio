const contactForm = document.querySelector('#contact-form');

const fields = [
    { id: 'name', message: 'Vul minimaal 2 tekens in.' },
    { id: 'email', message: 'Vul een geldig e-mailadres in.' },
    { id: 'message', message: 'Schrijf minimaal 10 tekens.' }
];

function validateField(field) {
    const input = document.querySelector(`#${field.id}`);
    const error = document.querySelector(`#${field.id}-error`);

    if (!input || !error) {
        return true;
    }

    const isValid = input.checkValidity();
    input.setAttribute('aria-invalid', String(!isValid));
    error.textContent = isValid ? '' : field.message;

    return isValid;
}

function validateForm() {
    return fields.map(validateField).every(Boolean);
}

if (contactForm) {
    fields.forEach(field => {
        const input = document.querySelector(`#${field.id}`);

        if (input) {
            input.addEventListener('blur', () => validateField(field));
            input.addEventListener('input', () => {
                if (input.getAttribute('aria-invalid') === 'true') {
                    validateField(field);
                }
            });
        }
    });

    contactForm.addEventListener('submit', event => {
        event.preventDefault();
        const status = document.querySelector('#form-status');

        if (!validateForm()) {
            status.textContent = 'Controleer de ingevulde velden.';
            return;
        }

        status.textContent = 'Formulier correct ingevuld. Bedankt!';
        contactForm.reset();

        fields.forEach(field => {
            const input = document.querySelector(`#${field.id}`);
            if (input) {
                input.setAttribute('aria-invalid', 'false');
            }
        });
    });
}
