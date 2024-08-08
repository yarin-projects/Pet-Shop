
document.addEventListener('DOMContentLoaded', () => {
    const nameInput = document.getElementById('Name');
    const ageInput = document.getElementById('Age');
    const pictureInput = document.getElementById('PictureName');
    const descriptionInput = document.getElementById('Description');
    const categoryInput = document.getElementById('CategoryId');

    const form = document.getElementById('animalForm');
    const btnSubmit = document.getElementById('btnSubmit');

    form.addEventListener('submit', event => {
        event.preventDefault();
        btnSubmit.disabled = true;
        if (validateNameInput(event, nameInput, btnSubmit) === false ||
            validateAgeInput(event, ageInput, btnSubmit) === false ||
            validatePictureInput(event, pictureInput, btnSubmit, true) === false ||
            validateDescriptionInput(event, descriptionInput, btnSubmit) === false ||
            validateCategoryInput(event, categoryInput, btnSubmit) === false) {
            event.stopPropagation();
            btnSubmit.disabled = false;
            form.classList.add('was-validated');
            return;
        }

        if (!form.checkValidity()) {
            event.stopPropagation();
            btnSubmit.disabled = false;
            form.classList.add('was-validated');
            return;
        }
        form.classList.add('was-validated');
        const formData = new FormData(form);
        const config = {
            headers: {
                'Content-Type': 'multipart/form-data',
            }
        };

        axios.put(`http://localhost:5065/admin/edit`, formData, config)
            .then(response => {
                btnSubmit.disabled = false
                if (response.status === 201) {
                    window.location.href = '/admin';
                }
            })
            .catch(() => btnSubmit.disabled = false);
    }, false);
});
