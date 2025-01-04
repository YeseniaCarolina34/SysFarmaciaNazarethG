

document.getElementById("saveProveedorBtn").addEventListener("click", function () {
    const form = document.getElementById("addProveedorForm");
    const formData = new FormData(form);

    fetch('/Proveedor/Create', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (response.ok) {
                response.json().then(data => {
                    alert("Proveedor agregado exitosamente.");
                    // Opcional: agregar el proveedor recién creado al dropdown sin recargar
                    const proveedorDropdown = document.querySelector('select[asp-for="IdProveedor"]');
                    const newOption = new Option(data.nombre, data.id, false, false);
                    proveedorDropdown.add(newOption);
                    proveedorDropdown.value = data.id; // Selecciona automáticamente el nuevo proveedor
                });
                // Cerrar el modal
                const modal = bootstrap.Modal.getInstance(document.getElementById("addProveedorModal"));
                modal.hide();
            } else {
                response.json().then(data => {
                    alert(`Error: ${data.message || "No se pudo guardar el proveedor."}`);
                });
            }
        })
        .catch(error => {
            console.error("Error al guardar el proveedor:", error);
            alert("Error al procesar la solicitud.");
        });
});
