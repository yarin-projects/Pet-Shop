const deleteAnimal = async (animalId) => {
    const btn = document.getElementById(`btnDeleteAnimal-${animalId}`);
    btn.disabled = true;
    try {
        await axios.delete(`http://localhost:5065/admin/deleteanimal`, {
            params: {
                animalId: Number(animalId)
            }
        });
        btn.disabled = false;
        location.reload();
    } catch (error) {
        btn.disabled = false;
    }
}

const deleteEmptyCategories = async () => {
    const btn = document.getElementById('btnDeleteEmptyCategories');
    btn.disabled = true;
    try {
        await axios.delete(`http://localhost:5065/admin/deleteemptycategories`);
        showToast();
        setTimeout(() => {
            location.reload();
            btn.disabled = false;
        }, 2500);
    } catch (error) {
        btn.disabled = false;
    }
}

const toastDiv = document.getElementById('liveToast');
const toast = bootstrap.Toast.getOrCreateInstance(toastDiv);
const showToast = () => {
    toast.show();
}