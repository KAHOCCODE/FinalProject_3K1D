using FinalProject_3K1D.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace _3K1D_Final.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeAdminController : Controller
    {

        [Route("Admin/HomeAdmin")]
        [Route("Admin/")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
