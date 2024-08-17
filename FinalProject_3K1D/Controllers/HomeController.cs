using FinalProject_3K1D.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FinalProject_3K1D.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            using (var db = new QlrapPhimContext())
            {
                var phims = db.Phims
                    .Include(p => p.IdTheLoais)
                    .ToList();
                return View(phims);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Detail()
        {
            var id = Request.Query["id"].ToString();
            using (var db = new QlrapPhimContext())
            {
                var phim = db.Phims
                    .Include(p => p.IdTheLoais)
                    .Include(p => p.LichChieus)
                    .FirstOrDefault(p => p.IdPhim == id);
                return View(phim);
            }
        }
        public IActionResult _Home()
        {
            using (var db = new QlrapPhimContext())
            {
                var phims = db.Phims
                    .Include(p => p.IdTheLoais)
                    .ToList();
                return View(phims);
            }
        }
        public IActionResult ChonGhe()
        {
            ////lấy tên phim, giờ chiếu
            //using (var db = new QlrapPhimContext())
            //{
            //    var phim = db.Phims
            //        .FirstOrDefault(p => p.IdPhim == phimId.ToString());
            //    ViewBag.TenPhim = phim.TenPhim;
            //    ViewBag.GioChieu = gioChieu;
            //}


            return View();

        }
}
}
