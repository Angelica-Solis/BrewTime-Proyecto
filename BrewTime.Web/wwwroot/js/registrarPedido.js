document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("pedidoForm");
    if (!form) return;

    if (window.jQuery?.validator) {
        $.extend($.validator.messages, {
            required: "Este campo es obligatorio.",
            number: "Ingrese un número válido",
            digits: "Ingrese solamente números"
        });
    }

    const subtotal = Number(form.dataset.subtotal || 0);
    const impuesto = Number(form.dataset.impuesto || 0);
    const minutosLabel = form.dataset.minutos || "minutos";

    const metodoEntrega = document.getElementById("metodoEntrega");
    const direccionContainer = document.getElementById("direccionContainer");
    const direccionEntrega = document.getElementById("direccionEntrega");
    const sugerencias = document.getElementById("sugerenciasDireccion");
    const btnRuta = document.getElementById("btnCalcularRuta");
    const resultadoRuta = document.getElementById("resultadoRuta");
    const direccionEncontrada = document.getElementById("direccionEncontrada");
    const distanciaRuta = document.getElementById("distanciaRuta");
    const tiempoRuta = document.getElementById("tiempoRuta");
    const costoDistanciaRuta = document.getElementById("costoDistanciaRuta");

    const costoBaseTexto = document.getElementById("costoBaseTexto");
    const costoDistanciaTexto = document.getElementById("costoDistanciaTexto");
    const costoEnvioTexto = document.getElementById("costoEnvioTexto");
    const totalPedidoTexto = document.getElementById("totalPedidoTexto");

    const moneda = new Intl.NumberFormat("es-CR", {
        style: "currency",
        currency: "CRC"
    });

    let rutaCalculada = false;
    let enviando = false;
    let timerBusqueda;

    const opcionActual = () =>
        metodoEntrega?.options[metodoEntrega.selectedIndex];

    const esDomicilio = () =>
        opcionActual()?.dataset.domicilio === "true" ||
        opcionActual()?.textContent.toLowerCase().includes("domicilio");

    const costoBase = () =>
        Number(opcionActual()?.dataset.costo || 0);

    function restaurarCosto() {
        const base = costoBase();

        if (costoBaseTexto)
            costoBaseTexto.textContent = moneda.format(base);

        if (costoDistanciaTexto)
            costoDistanciaTexto.textContent = moneda.format(0);

        if (costoEnvioTexto)
            costoEnvioTexto.textContent = moneda.format(base);

        if (totalPedidoTexto)
            totalPedidoTexto.textContent =
                moneda.format(subtotal + impuesto + base);
    }

    function actualizarEntrega() {
        const domicilio = esDomicilio();

        if (direccionContainer)
            direccionContainer.hidden = !domicilio;

        if (direccionEntrega) {
            direccionEntrega.required = domicilio;

            if (!domicilio)
                direccionEntrega.value = "";
        }

        rutaCalculada = false;

        if (resultadoRuta)
            resultadoRuta.hidden = true;

        if (sugerencias)
            sugerencias.hidden = true;

        restaurarCosto();
    }

    metodoEntrega?.addEventListener("change", actualizarEntrega);
    actualizarEntrega();

    async function buscarDirecciones(texto) {
        try {
            const datos = await $.ajax({
                url: form.dataset.urlDirecciones,
                type: "GET",
                data: { texto }
            });

            sugerencias.innerHTML = "";

            if (!datos.length) {
                sugerencias.hidden = true;
                return;
            }

            datos.forEach(item => {
                const boton = document.createElement("button");

                boton.type = "button";
                boton.className = "list-group-item list-group-item-action";
                boton.textContent = item.direccion;

                boton.addEventListener("click", () => {
                    direccionEntrega.value = item.direccion;
                    sugerencias.innerHTML = "";
                    sugerencias.hidden = true;
                    rutaCalculada = false;
                    resultadoRuta.hidden = true;
                    restaurarCosto();
                });

                sugerencias.appendChild(boton);
            });

            sugerencias.hidden = false;
        }
        catch {
            sugerencias.hidden = true;
        }
    }

    direccionEntrega?.addEventListener("input", () => {
        rutaCalculada = false;

        if (resultadoRuta)
            resultadoRuta.hidden = true;

        restaurarCosto();
        clearTimeout(timerBusqueda);

        const texto = direccionEntrega.value.trim();

        if (texto.length < 3) {
            sugerencias.innerHTML = "";
            sugerencias.hidden = true;
            return;
        }

        timerBusqueda = setTimeout(
            () => buscarDirecciones(texto),
            400
        );
    });

    async function calcularRuta() {
        const direccion = direccionEntrega?.value.trim();

        if (!direccion) {
            Swal.fire(
                "Dirección requerida",
                "Debe seleccionar o ingresar una dirección.",
                "warning"
            );
            return;
        }

        if (sugerencias)
            sugerencias.hidden = true;

        btnRuta.disabled = true;
        btnRuta.textContent = "Calculando...";

        const token = form.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value;

        try {
            const response = await $.ajax({
                url: form.dataset.urlRuta,
                type: "POST",
                data: {
                    direccion,
                    __RequestVerificationToken: token
                }
            });

            if (!response.ok) {
                rutaCalculada = false;
                Swal.fire("Error", response.mensaje, "error");
                return;
            }

            const base = costoBase();
            const adicional =
                Number(response.costoPorDistancia || 0);

            const envio = base + adicional;
            const total = subtotal + impuesto + envio;

            direccionEncontrada.textContent =
                response.direccionEncontrada;

            distanciaRuta.textContent =
                `${response.distanciaKilometro} km`;

            tiempoRuta.textContent =
                `${response.tiempoEstimado} ${minutosLabel}`;

            costoDistanciaRuta.textContent =
                moneda.format(adicional);

            if (costoBaseTexto)
                costoBaseTexto.textContent =
                    moneda.format(base);

            if (costoDistanciaTexto)
                costoDistanciaTexto.textContent =
                    moneda.format(adicional);

            if (costoEnvioTexto)
                costoEnvioTexto.textContent =
                    moneda.format(envio);

            if (totalPedidoTexto)
                totalPedidoTexto.textContent =
                    moneda.format(total);

            resultadoRuta.hidden = false;
            rutaCalculada = true;
        }
        catch {
            rutaCalculada = false;
            console.error("Error al calcular la ruta:", error);

            Swal.fire(
                "Error",
                "No fue posible calcular la ruta.",
                "error"
            );
        }
        finally {
            btnRuta.disabled = false;
            btnRuta.textContent = "Calcular entrega";
        }
    }

    btnRuta?.addEventListener("click", calcularRuta);

    const clienteSelect = document.getElementById("clienteSelect");

    if (clienteSelect) {
        const nombre = document.getElementById("clienteNombre");
        const correo = document.getElementById("clienteCorreo");
        const telefono = document.getElementById("clienteTelefono");

        function actualizarCliente() {
            const opcion =
                clienteSelect.options[clienteSelect.selectedIndex];

            if (nombre)
                nombre.textContent =
                    opcion?.dataset.nombre || "Seleccione un cliente";

            if (correo)
                correo.textContent =
                    opcion?.dataset.correo || "—";

            if (telefono)
                telefono.textContent =
                    opcion?.dataset.telefono || "No registrado";
        }

        clienteSelect.addEventListener("change", actualizarCliente);
        actualizarCliente();
    }

    form.addEventListener("submit", async event => {
        event.preventDefault();

        if (enviando) return;

        if (window.jQuery?.validator && !$(form).valid())
            return;

        if (!form.checkValidity()) {
            form.reportValidity();
            return;
        }

        if (esDomicilio() && !rutaCalculada) {
            Swal.fire(
                "Calcule la entrega",
                "Debe calcular la ruta antes de registrar el pedido.",
                "warning"
            );
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

        form.submit();
    });
});