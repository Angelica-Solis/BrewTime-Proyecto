document.addEventListener("DOMContentLoaded", () => {
    const formulario = document.getElementById("pagoForm");
    const totalPedido = Number(formulario.dataset.total);

    const radios = document.querySelectorAll(
        'input[name="Pago.MetodoPagoId"]'
    );

    const tarjeta = document.getElementById("tarjetaContainer");
    const efectivo = document.getElementById("efectivoContainer");

    const titular = document.getElementById("nombreTitular");
    const numero = document.getElementById("numeroTarjeta");
    const vencimiento = document.getElementById("fechaVencimiento");
    const codigo = document.getElementById("codigoSeguridad");

    const monto = document.getElementById("montoPagado");
    const vueltoTexto = document.getElementById("vueltoTexto");
    const vueltoHidden = document.getElementById("vueltoHidden");
    const mensajeMonto = document.getElementById("mensajeMonto");

    const moneda = new Intl.NumberFormat("es-CR", {
        style: "currency",
        currency: "CRC"
    });

    function actualizarMetodo() {
        const seleccionado = document.querySelector(
            'input[name="Pago.MetodoPagoId"]:checked'
        );

        const tipo = seleccionado?.dataset.tipo ?? "";
        const esTarjeta = tipo === "tarjeta";
        const esEfectivo = tipo === "efectivo";

        tarjeta.hidden = !esTarjeta;
        efectivo.hidden = !esEfectivo;

        titular.required = esTarjeta;
        numero.required = esTarjeta;
        vencimiento.required = esTarjeta;
        codigo.required = esTarjeta;
        monto.required = esEfectivo;

        if (esEfectivo) {
            calcularVuelto();
        } else {
            vueltoHidden.value = "";
        }
    }

    function calcularVuelto() {
        const montoRecibido = Number(monto.value || 0);
        const diferencia = montoRecibido - totalPedido;

        if (montoRecibido <= 0) {
            vueltoTexto.textContent = moneda.format(0);
            vueltoHidden.value = "";
            mensajeMonto.textContent = "Ingrese el monto recibido.";
            mensajeMonto.classList.remove("monto-error");
            return;
        }

        if (diferencia < 0) {
            vueltoTexto.textContent = moneda.format(0);
            vueltoHidden.value = "";

            mensajeMonto.textContent =
                `Faltan ${moneda.format(Math.abs(diferencia))}.`;

            mensajeMonto.classList.add("monto-error");
            return;
        }

        vueltoTexto.textContent = moneda.format(diferencia);
        vueltoHidden.value = diferencia.toFixed(2);

        mensajeMonto.textContent =
            diferencia === 0
                ? "Monto exacto."
                : "Vuelto calculado automáticamente.";

        mensajeMonto.classList.remove("monto-error");
    }

    radios.forEach(radio => {
        radio.addEventListener("change", actualizarMetodo);
    });

    monto.addEventListener("input", calcularVuelto);

    numero.addEventListener("input", () => {
        numero.value = numero.value.replace(/\D/g, "");
    });

    codigo.addEventListener("input", () => {
        codigo.value = codigo.value.replace(/\D/g, "");
    });

    vencimiento.addEventListener("input", () => {
        let valor = vencimiento.value
            .replace(/\D/g, "")
            .slice(0, 4);

        if (valor.length > 2) {
            valor = `${valor.slice(0, 2)}/${valor.slice(2)}`;
        }

        vencimiento.value = valor;
    });

    actualizarMetodo();
});