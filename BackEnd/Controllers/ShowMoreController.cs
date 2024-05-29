using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    public class ShowMoreController : Controller
    {
        public IActionResult Index()
        {
            return View("ShowMore");
        }
    }
}
