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
        //delete phim
        [HttpPost]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID phim không hợp lệ!";
                return RedirectToAction("Index");
            }

            try
            {
                var movie = _context.Phims.Find(id);
                if (movie != null)
                {
                    _context.Phims.Remove(movie);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Phim đã được xóa thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy phim cần xóa!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa phim: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
        public IActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID phim không hợp lệ!";
                return RedirectToAction("Index");
            }

            var movie = _context.Phims.Find(id);
            if (movie == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phim!";
                return RedirectToAction("Index");
            }

            return View(movie);
        }
        //edit phim
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID phim không hợp lệ!";
                return RedirectToAction("Index");
            }

            var movie = _context.Phims.Find(id);
            if (movie == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phim!";
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Phim phim)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Phims.Update(phim);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Phim đã được cập nhật thành công!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi cập nhật phim: {ex.Message}";
            }

            return View(phim);
        }























    }
}