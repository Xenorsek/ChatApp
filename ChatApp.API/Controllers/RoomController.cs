using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    public class RoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
