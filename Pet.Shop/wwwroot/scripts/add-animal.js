document.addEventListener('DOMContentLoaded', () => {
    const nameInput = document.getElementById('Name');
    const ageInput = document.getElementById('Age');
    const pictureInput = document.getElementById('PictureName');
    const descriptionInput = document.getElementById('Description');
    const categoryInput = document.getElementById('CategoryId');

    const form = document.getElementById('animalForm');
    const btn = document.getElementById('btnSubmit');
    form.addEventListener('submit', event => {
        btn.disabled = true;

        validateNameInput(event, nameInput, btn);
        validateAgeInput(event, ageInput, btn);
        validatePictureInput(event, pictureInput, btn, false);
        validateDescriptionInput(event, descriptionInput, btn);
        validateCategoryInput(event, categoryInput, btn);

        if (!form.checkValidity()) {
            btn.disabled = false;
            stopEvent(event);
        }

        form.classList.add('was-validated');
    }, false);

}, false);