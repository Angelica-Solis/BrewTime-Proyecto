document.addEventListener("DOMContentLoaded", () => {
    const formulario = document.getElementById("pagoForm");
    if (!formulario) return;

    // Validaciones de jQuery en español
    if (window.jQuery?.validator) {
        formulario.setAttribute("novalidate", "novalidate");

        $.extend($.validator.messages, {
            required: "Este campo es obligatorio",
            email: "Ingrese un correo electrónico válido",
            number: "Ingrese un número válido",
            digits: "Ingrese solamente números",
            minlength: $.validator.format(
                "Debe ingresar al menos {0} caracteres"
            ),
            maxlength: $.validator.format(
                "No puede ingresar más de {0} caracteres"
            ),
            range: $.validator.format(
                "Ingrese un valor entre {0} y {1}"
            ),
            min: $.validator.format(
                "Ingrese un valor mayor o igual a {0}"
            ),
            max: $.validator.format(
                "Ingrese un valor menor o igual a {0}"
            ),
            step: $.validator.format(
                "Ingrese un múltiplo de {0}"
            )
        });

        const validator = $(formulario).validate();

        validator.settings.messages["Pago.MetodoPagoId"] = {
            required: "Debe seleccionar un método de pago"
        };

        validator.settings.messages["Pago.NombreTitular"] = {
            required: "Debe ingresar el nombre del titular",
            maxlength: "El nombre no puede superar los 100 caracteres"
        };

        validator.settings.messages["Pago.NumeroTarjeta"] = {
            required: "Debe ingresar el número de tarjeta",
            digits: "El número de tarjeta debe contener solamente números",
            regex: "El número de tarjeta debe tener entre 13 y 19 dígitos"
        };

        validator.settings.messages["Pago.FechaVencimiento"] = {
            required: "Debe ingresar la fecha de vencimiento",
            regex: "Ingrese la fecha con el formato MM/AA"
        };

        validator.settings.messages["Pago.CodigoSeguridad"] = {
            required: "Debe ingresar el código de seguridad",
            digits: "El código de seguridad debe contener solamente números",
            regex: "El código de seguridad debe tener 3 o 4 dígitos"
        };

        validator.settings.messages["Pago.MontoPagado"] = {
            required: "Debe ingresar el monto recibido",
            number: "Ingrese un monto válido",
            min: "El monto recibido debe ser mayor a cero",
            range: "El monto recibido debe ser mayor a cero"
        };
    }

    const totalPedido = Number(formulario.dataset.total || 0);
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

    function limpiarValidacion(...campos) {
        campos.forEach(campo => {
            if (!campo) return;

            campo.classList.remove("input-validation-error");

            const mensaje = document.querySelector(
                `[data-valmsg-for="${campo.name}"]`
            );

            if (mensaje) {
                mensaje.textContent = "";
                mensaje.classList.remove("field-validation-error");
                mensaje.classList.add("field-validation-valid");
            }
        });
    }

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

        if (!esTarjeta) {
            limpiarValidacion(
                titular,
                numero,
                vencimiento,
                codigo
            );
        }

        if (esEfectivo) {
            calcularVuelto();
        } else {
            vueltoHidden.value = "";
            limpiarValidacion(monto);
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

        mensajeMonto.textContent = diferencia === 0
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