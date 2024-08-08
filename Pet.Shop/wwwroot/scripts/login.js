document.addEventListener('DOMContentLoaded', () => {
    const usernameInput = document.getElementById('Username');
    const passwordInput = document.getElementById('Password');

    const form = document.getElementById('loginForm');
    const btn = document.getElementById('btnSubmit');
    form.addEventListener('submit', event => {
        btn.disabled = true;

        validateUsernameInput(event, usernameInput, btn);
        validatePasswordInput(event, passwordInput, btn);

        if (!form.checkValidity()) {
            btn.disabled = false;
            stopEvent(event);
        }

        form.classList.add('was-validated');
    }, false);
}, false);