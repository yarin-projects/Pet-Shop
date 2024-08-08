
const editComment = async (commentId, animalId, isAnimalList) => {
    let comment, btn;
    if (isAnimalList) {
        comment = document.getElementById(`Content-B-${commentId}`);
        btn = document.getElementById(`btnEditComment-B-${commentId}`);
    }
    else {
        comment = document.getElementById(`Content-${commentId}`);
        btn = document.getElementById(`btnEditComment-${commentId}`);
    }
    btn.disabled = true;
    if (validateComment(comment) === false) {
        btn.disabled = false;
        return;
    }
    const content = comment.value.trim();
    try {
        await axios.put(`http://localhost:5065/account/editcomment`, {
            animalId,
            commentId,
            content
        });
        btn.disabled = false;
        location.reload();
    } catch (error) {
        btn.disabled = false;
    }
}

const deleteComment = async (commentId, isAnimalList) => {
    let btn;
    if (isAnimalList) {
        btn = document.getElementById(`btnDeleteComment-B-${commentId}`);
    }
    else {
        btn = document.getElementById(`btnDeleteComment-${commentId}`);
    }
    btn.disabled = true;
    try {
        await axios.delete(`http://localhost:5065/account/deletecomment`, {
            params: {
                commentId: Number(commentId)
            }
        });
        btn.disabled = false;
        location.reload();
    } catch (error) {
        btn.disabled = false;
    }
}
const deleteAccount = async (userId, isAdmin, adminId, btnId) => {
    let btn;
    if (btnId) {
        btn = document.getElementById(btnId);
    }
    else {
        btn = document.getElementById(`btnDeleteAccount-${userId}`);
    }
    btn.disabled = true;
    try {
        await axios.delete(`http://localhost:5065/account/deleteaccount`, {
            params: {
                userId: Number(userId)
            }
        });
        btn.disabled = false;
        if (isAdmin && userId !== adminId) {
            window.location.reload();
        }
        else {
            window.location.href = "/account/logout";
        }
    } catch (error) {
        btn.disabled = false;
    }
}

const deleteGuestComments = async (btnId) => {
    const btn = document.getElementById(`deleteGuestCommentsBtn-${btnId}`);
    btn.disabled = true;
    try {
        await axios.delete(`http://localhost:5065/admin/deleteguestcomments`);
        btn.disabled = false;
        window.location.reload();
    } catch (error) {
        btn.disabled = false;
    }
}

const deleteUserComments = async (userId) => {
    const btn = document.getElementById(`btnDeleteComments-${userId}`);
    btn.disabled = true;
    try {
        await axios.delete(`http://localhost:5065/account/DeleteUserComments`, {
            params: {
                userId: Number(userId)
            }
        });
        btn.disabled = false;
        window.location.reload();
    } catch (error) {
        btn.disabled = false;
    }
}