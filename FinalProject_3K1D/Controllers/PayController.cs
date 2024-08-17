using FinalProject_3K1D.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_3K1D.Controllers
{
    public class PayController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public PayController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {


            return View();
        }
    }
}
