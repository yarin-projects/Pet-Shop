const toggleSortOrder = () => {
    const isDescendingInput = document.getElementById("isDescending");
    isDescendingInput.value = (isDescendingInput.value === "true") ? "false" : "true";
    document.getElementById("filterForm").submit();
}

const submitForm = () => {
    document.getElementById("filterForm").submit();
}

const containsLettersOnly = (str) => {
    const regex = XRegExp('^[\\p{L}]+$', 'u');// Using XRegExp for better Unicode support
    return XRegExp.test(str, regex);
}
const stopEvent = event => {
    event.preventDefault();
    event.stopImmediatePropagation();
}

const isValidName = name => {
    const regex = XRegExp('^\\p{L}{3,15}$', 'u');  
    return XRegExp.test(name, regex);
}

const isValidAge = age => {
    return /^\d{1,2}$/.test(age) && age >= 1 && age <= 25;
}

const isValidUsername = username => {
    return /^[a-zA-Z0-9]{3,15}$/.test(username);
}
const isValidPassword = password => {
    return /^[a-zA-Z0-9!@]{3,25}$/.test(password);
}

const isValidDescription = description => {
    return description.length > 2 && description.length < 40;
}

const isValidCategoryName = name => {
    const regex = XRegExp('^\\p{L}{3,25}$', 'u');
    return XRegExp.test(name, regex);
};

const validateUsernameInput = (event, usernameInput, btn) => {
    if (!isValidUsername(usernameInput.value)) {
        usernameInput.classList.add('is-invalid');
        usernameInput.setCustomValidity('Username must be between 3 and 15 characters and contain only English letters and digits.');
        btn.disabled = false;
        stopEvent(event);
        return false;
    }
    else {
        usernameInput.classList.remove('is-invalid');
        usernameInput.setCustomValidity('');
        return true;
    }
}

const validatePasswordConfirm = (event, passwordInput, passwordConfirmInput, btn) => {
    if (passwordConfirmInput.value === passwordInput.value) {
        passwordInput.classList.remove('is-invalid');
        passwordInput.setCustomValidity('');
        passwordConfirmInput.classList.remove('is-invalid');
        passwordConfirmInput.setCustomValidity('');
        return true;
    }
    else {
        passwordInput.setCustomValidity('Password and confirm password must be the same.');
        passwordConfirmInput.setCustomValidity('Password and confirm password must be the same.');
        btn.disabled = false;
        stopEvent(event);
        return false;
    }
}

const validatePasswordInput = (event, passwordInput, btn) => {
    if (!isValidPassword(passwordInput.value)) {
        passwordInput.classList.add('is-invalid');
        passwordInput.setCustomValidity('Password must be between 3 and 25 characters and contain only English letters, digits, and the special characters "!@".');
        btn.disabled = false;
        stopEvent(event);
        return false;
    }
    else {
        passwordInput.classList.remove('is-invalid');
        passwordInput.setCustomValidity('');
        return true;
    }
}

const validateNameInput = (event, nameInput, btn) => {
    if (!isValidName(nameInput.value)) {
        nameInput.classList.add('is-invalid');
        nameInput.setCustomValidity('Please enter a valid name (letters only, 3-15 characters).');
        btn.disabled = false;
        stopEvent(event);
        return false;
    } else {
        nameInput.classList.remove('is-invalid');
        nameInput.setCustomValidity('');
    }
}
const validateAgeInput = (event, ageInput, btn) => {
    if (!isValidAge(ageInput.value)) {
        ageInput.classList.add('is-invalid');
        ageInput.classList.remove('is-valid');
        btn.disabled = false;
        stopEvent(event);
        return false;
    } else {
        ageInput.classList.remove('is-invalid');
        ageInput.classList.add('is-valid');
    }
}
const validatePictureInput = (event, pictureInput, btn, isEdit) => {
    if (pictureInput.files.length > 0) {
        const file = pictureInput.files[0];
        const fileType = file.type;
        if (!fileType.startsWith('image/')) {
            pictureInput.classList.add('is-invalid');
            pictureInput.setCustomValidity('Please upload a valid image file.');
            btn.disabled = false;
            stopEvent(event);
            return false;
        } else {
            pictureInput.classList.remove('is-invalid');
            pictureInput.setCustomValidity('');
        }
    } else if (!isEdit) {
        pictureInput.classList.add('is-invalid');
        pictureInput.setCustomValidity('Please upload a valid image file.');
        btn.disabled = false;
        stopEvent(event);
        return false;
    }
}
const validateDescriptionInput = (event, descriptionInput, btn) => {
    if (!isValidDescription(descriptionInput.value)) {
        descriptionInput.classList.add('is-invalid');
        descriptionInput.setCustomValidity('Description must be between 3 and 40 characters long.');
        btn.disabled = false;
        stopEvent(event);
        return false;
    } else {
        descriptionInput.classList.remove('is-invalid');
        descriptionInput.setCustomValidity('');
    }
}
const validateCategoryInput = (event, categoryInput, btn) => {
    if (!categoryInput.value) {
        categoryInput.classList.add('is-invalid');
        categoryInput.classList.remove('is-valid');
        btn.disabled = false;
        stopEvent(event);
        return false;
    } else {
        categoryInput.classList.remove('is-invalid');
        categoryInput.classList.add('is-valid');
    }
}

const validateComment = comment => {
    const commentValue = comment.value.trim()
    const isValid = commentValue.length > 2 && commentValue.length < 101;

    if (!isValid) {
        comment.classList.add('is-invalid');
        comment.setCustomValidity('Comment must be between 2 and 100 characters and not just whitespace.');
    } else {
        comment.classList.remove('is-invalid');
        comment.setCustomValidity('');
    }

    return isValid;
};