using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrewTime.Web.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly IServiceChatBot _service;
        public ChatBotController(IServiceChatBot service)
        {
            _service = service;
        }



        public IActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Send(
        [FromBody] ChatRequestDTO dto)
        {

            var response =
            await _service.SendMessageAsync(dto);


            return Json(response);

        }
    }
}