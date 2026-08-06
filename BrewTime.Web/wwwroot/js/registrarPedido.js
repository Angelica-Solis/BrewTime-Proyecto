document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("pedidoForm");
    if (!form) return;

    //mensajes generales de jQuery Validate 
    if (window.jQuery?.validator) {
        $.extend($.validator.messages, {
            required: "Este campo es obligatorio.",
            number: "Ingrese un número válido",
            digits: "Ingrese solamente números",
            maxlength: $.validator.format(
                "No puede ingresar más de {0} caracteres"
            ),
            minlength: $.validator.format(
                "Debe ingresar al menos {0} caracteres"
            ),
            range: $.validator.format(
                "Ingrese un valor entre {0} y {1}"
            )
        });
    }

    const subtotal = Number(form.dataset.subtotal || 0);
    const impuesto = Number(form.dataset.impuesto || 0);

    const metodoEntrega = document.getElementById("metodoEntrega");
    const direccionContainer = document.getElementById("direccionContainer");
    const direccionEntrega = document.getElementById("direccionEntrega");
    const costoEnvioTexto = document.getElementById("costoEnvioTexto");
    const totalPedidoTexto = document.getElementById("totalPedidoTexto");

    const moneda = new Intl.NumberFormat("es-CR", {
        style: "currency",
        currency: "CRC"
    });

    function actualizarEntrega() {
        if (!metodoEntrega) return;

        const opcion = metodoEntrega.options[metodoEntrega.selectedIndex];
        const costo = Number(opcion?.dataset.costo || 0);
        const esDomicilio = opcion?.dataset.domicilio === "true";

        if (direccionContainer)
            direccionContainer.hidden = !esDomicilio;

        if (direccionEntrega) {
            direccionEntrega.required = esDomicilio;

            if (esDomicilio) {
                direccionEntrega.setAttribute(
                    "data-msg-required",
                    "Debe ingresar la dirección de entrega."
                );
            } else {
                direccionEntrega.value = "";
                direccionEntrega.classList.remove("input-validation-error");

                if (window.jQuery)
                    $(direccionEntrega).next(".field-validation-error").empty();
            }
        }

        if (costoEnvioTexto)
            costoEnvioTexto.textContent = moneda.format(costo);

        if (totalPedidoTexto)
            totalPedidoTexto.textContent =
                moneda.format(subtotal + impuesto + costo);
    }

    if (metodoEntrega) {
        metodoEntrega.addEventListener("change", actualizarEntrega);
        actualizarEntrega();
    }

    const clienteSelect = document.getElementById("clienteSelect");

    if (clienteSelect) {
        const clienteNombre = document.getElementById("clienteNombre");
        const clienteCorreo = document.getElementById("clienteCorreo");
        const clienteTelefono = document.getElementById("clienteTelefono");

        function actualizarCliente() {
            const opcion =
                clienteSelect.options[clienteSelect.selectedIndex];

            if (clienteNombre)
                clienteNombre.textContent =
                    opcion?.dataset.nombre || "Seleccione un cliente";

            if (clienteCorreo)
                clienteCorreo.textContent =
                    opcion?.dataset.correo || "—";

            if (clienteTelefono)
                clienteTelefono.textContent =
                    opcion?.dataset.telefono || "No registrado";
        }

        clienteSelect.addEventListener("change", actualizarCliente);
        actualizarCliente();
    }

    let enviando = false;

    form.addEventListener("submit", async event => {
        event.preventDefault();

        if (enviando) return;

        //validaciones de ASP.NET Core y jQuery Validate
        if (window.jQuery?.validator && !$(form).valid())
            return;

        //validaciones HTML como required
        if (!form.checkValidity()) {
            form.reportValidity();
            return;
        }

        const resultado = await Swal.fire({
            title: "¿Registrar el pedido?",
            text: "El pedido se guardará como pendiente de pago.",
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Sí, registrar",
            cancelButtonText: "No, revisar",
            confirmButtonColor: "#1f4a2e",
            cancelButtonColor: "#6f4e37",
            reverseButtons: true,
            allowOutsideClick: false
        });

        if (!resultado.isConfirmed) return;

        enviando = true;

        Swal.fire({
            title: "Registrando pedido...",
            text: "Por favor espere.",
            allowOutsideClick: false,
            allowEscapeKey: false,
            showConfirmButton: false,
            didOpen: () => Swal.showLoading()
        });

        //envía sin volver a ejecutar el evento submit
        form.submit();
    });
});