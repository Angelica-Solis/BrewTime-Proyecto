 document.addEventListener("DOMContentLoaded", function () {

    const brewChatbot = document.getElementById("brew-chatbot");
    const btnAbrir = document.getElementById("btnAbrirChat");
    const btnCerrar = document.getElementById("btnCerrarChat");

    if (!brewChatbot || !btnAbrir || !btnCerrar)
        return;

    btnAbrir.addEventListener("click", function () {
        brewChatbot.classList.add("chat-open");
    });

    btnCerrar.addEventListener("click", function () {
        brewChatbot.classList.remove("chat-open");
    });
     const botonesRapidos =
         document.querySelectorAll(".quick-question");


     botonesRapidos.forEach(btn => {

         btn.addEventListener("click", function () {

             document.getElementById("txtPregunta").value =
                 this.textContent;


             enviarMensaje();

         });

     });
 });
 // boton enviar
async function enviarMensaje() {

    console.log("Entró enviarMensaje");


    let input = document.getElementById("txtPregunta");

    let mensaje = input.value.trim();


    console.log("Mensaje enviado:", mensaje);


    if (mensaje === "") {
        return;
    }


    document.getElementById("chatMessages")
        .innerHTML +=
        `
    <div class="message user">
        <div class="message-content">
            ${mensaje}
        </div>
    </div>
    `;

    scrollChatBottom();

    let response =
        await fetch('/ChatBot/Send',
            {
                method: 'POST',

                headers:
                {
                    'Content-Type': 'application/json'
                },

                body:
                    JSON.stringify(
                        {
                            Message: mensaje
                        })
            });


    console.log("Status:", response.status);


    let data =
        await response.json();


    console.log(data);



    document.getElementById("chatMessages")
        .innerHTML +=
        `
    <div class="message bot">
        <div class="message-content">
            ${data.response}
        </div>
    </div>
    `;
    scrollChatBottom();

    input.value = "";

}
function scrollChatBottom() {

    const chatBody =
        document.querySelector(".chat-body");


    chatBody.scrollTop =
        chatBody.scrollHeight;

}