
document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('commentForm');
    const btn = document.getElementById('btnSubmit');
    const comment = document.getElementById('Content');

    form.addEventListener('submit', event => {
        btn.disabled = true;
        const isCommentValid = validateComment(comment);
        const isValid = form.checkValidity();
        if (!isValid || !isCommentValid) {
            btn.disabled = false;
            stopEvent(event);
        }

        form.classList.add('was-validated');
    }, false);

    
});
