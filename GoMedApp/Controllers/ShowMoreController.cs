using Microsoft.AspNetCore.Mvc;

namespace GoMedApp.Controllers
{
    public class ShowMoreController : Controller
    {
        [HttpGet]
        public IActionResult ShowMore()
        {
            return View("ShowMore");
        }

    }
}
