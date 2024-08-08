document.addEventListener('DOMContentLoaded', () => {
    const usernameInput = document.getElementById('Username');
    const oldPasswordInput = document.getElementById('OldPassword');
    const newPasswordInput = document.getElementById('NewPassword');
    const confirmNewPasswordInput = document.getElementById('ConfirmNewPassword');

    const form = document.getElementById('updateAccountForm');
    const btn = document.getElementById('btnSubmit');
    const toastTxt = document.getElementById('toastText');
    const serverValidations = document.querySelectorAll('.server-validation')
    form.addEventListener('submit', event => {
        serverValidations.forEach(serverValidation => serverValidation.textContent = '');
        btn.disabled = true;

        let isValid = true;
        if (oldPasswordInput.value || newPasswordInput.value || confirmNewPasswordInput.value) {
            isValid &= validatePasswordInput(event, oldPasswordInput, btn);
            isValid &= validatePasswordInput(event, newPasswordInput, btn);
            isValid &= validatePasswordInput(event, confirmNewPasswordInput, btn);
            isValid &= validatePasswordConfirm(event, newPasswordInput, confirmNewPasswordInput, btn);
            if (oldPasswordInput.value === newPasswordInput.value) {
                btn.disabled = false;
                showToast('Enter a different password if you would like to update it.');
                newPasswordInput.classList.add('is-invalid');
                newPasswordInput.setCustomValidity('is-invalid');
                confirmNewPasswordInput.classList.add('is-invalid');
                confirmNewPasswordInput.setCustomValidity('is-invalid');
                isValid = false;
            }
            else {
                newPasswordInput.classList.remove('is-invalid');
                newPasswordInput.setCustomValidity('');
                confirmNewPasswordInput.classList.remove('is-invalid');
                confirmNewPasswordInput.setCustomValidity('');
            }
        }
        isValid &= validateUsernameInput(event, usernameInput, btn);

        if (!isValid || !form.checkValidity()) {
            btn.disabled = false;
            event.preventDefault();
            event.stopImmediatePropagation();
            return;
        }
        form.classList.add('was-validated');
    }, false);
    const toastDiv = document.getElementById('liveToast');
    const toast = bootstrap.Toast.getOrCreateInstance(toastDiv);
    const showToast = text => {
        toastTxt.textContent = text;
        toast.show();
    }
}, false);