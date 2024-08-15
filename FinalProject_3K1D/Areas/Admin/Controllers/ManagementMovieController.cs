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
        //danh sách các phim đang chiếu
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

        //thêm phim mới 
        public IActionResult Create()
        {
            var nextId = GeneratePhimId();
            ViewData["NextId"] = nextId;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Phim phim)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    phim.IdPhim = GeneratePhimId(); // Generate IdPhim
                    _context.Phims.Add(phim);
                    _context.SaveChanges();

                    // Store success message in TempData
                    TempData["SuccessMessage"] = "Phim đã được thêm thành công!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                // Store error message in TempData
                TempData["ErrorMessage"] = $"Lỗi khi thêm phim: {ex.Message}";
            }

            return View(phim);
        }
        private string GeneratePhimId()
        {
            // Logic to generate the next IdPhim based on the last IdPhim in the database
            var lastPhim = _context.Phims.OrderByDescending(p => p.IdPhim).FirstOrDefault();
            if (lastPhim != null)
            {
                int nextId = int.Parse(lastPhim.IdPhim.Substring(1)) + 1;
                return $"P{nextId:D2}";
            }
            return "P01";
        }


    }
}
