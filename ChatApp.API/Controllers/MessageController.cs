using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    public class MessageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
