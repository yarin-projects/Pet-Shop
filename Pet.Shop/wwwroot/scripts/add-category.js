document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('categoryForm');
    const btn = document.getElementById('btnSubmit');
    form.addEventListener('submit', event => {
        btn.disabled = true;
        if (!form.checkValidity() || !validateForm()) {
            btn.disabled = false;
            stopEvent(event);
        }
        form.classList.add('was-validated');
    }, false);

    const validateForm = () => {
        const nameInput = document.getElementById('Name');
        const nameValue = nameInput.value.trim();

        if (!isValidCategoryName(nameValue)) {
            nameInput.classList.add('is-invalid');
            nameInput.classList.remove('is-valid');
            return false;
        } else {
            nameInput.classList.remove('is-invalid');
            nameInput.classList.add('is-valid');
            return true;
        }
    };
}, false);