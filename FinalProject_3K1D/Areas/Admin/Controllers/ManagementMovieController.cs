using FinalProject_3K1D.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace _3K1D_Final.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManagementMovieController : Controller
    {
        private readonly QlrapPhimContext _context;

        public ManagementMovieController(QlrapPhimContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var today =DateOnly.FromDateTime(DateTime.Now);
            var movies = _context.Phims
                .Where(p => p.NgayKhoiChieu <= today && p.NgayKetThuc >=today)
                .ToList();
            return View(movies);
        }
        public IActionResult MoviesEnd()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var movies = _context.Phims
                .Where(p => p.NgayKetThuc < today)
                .ToList();
            return View(movies);
        }
        

    }
}
