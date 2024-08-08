document.addEventListener('DOMContentLoaded', () => {
    const usernameInput = document.getElementById('Username');
    const passwordInput = document.getElementById('Password');
    const passwordConfirmInput = document.getElementById('PasswordConfirm');

    const form = document.getElementById('registerForm');
    const btn = document.getElementById('btnSubmit');
    form.addEventListener('submit', event => {
        btn.disabled = true;

        validateUsernameInput(event, usernameInput, btn);
        validatePasswordInput(event, passwordInput, btn);
        validatePasswordInput(event, passwordConfirmInput, btn);
        validatePasswordConfirm(event, passwordInput, passwordConfirmInput, btn);

        if (!form.checkValidity()) {
            btn.disabled = false;
            stopEvent(event);
        }

        form.classList.add('was-validated');
    }, false);
}, false);