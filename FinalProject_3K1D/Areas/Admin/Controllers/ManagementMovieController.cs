using FinalProject_3K1D.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        # region danh sách các phim đang chiếu
        public IActionResult Index(string searchString)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var movies = _context.Phims
                .Include(p => p.IdTheLoais)
                .Where(p => p.NgayKhoiChieu <= today && p.NgayKetThuc >= today);

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(p => p.TenPhim.Contains(searchString));
            }

            ViewData["SearchString"] = searchString;  

            return View(movies.ToList());
        }
        #endregion

        #region Phim đã kết thúc
        public IActionResult MoviesEnd(string searchString)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var movies = _context.Phims
                .Include(p => p.IdTheLoais)
                .Where(p => p.NgayKetThuc < today);

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(p => p.TenPhim.Contains(searchString));
            }

            ViewData["SearchString"] = searchString;  

            return View(movies.ToList());
        }
        #endregion

        #region Phim sắp chiếu
        public IActionResult MoviesComing(string searchString)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var movies = _context.Phims
                .Include(p => p.IdTheLoais)
                .Where(p => p.NgayKhoiChieu > today);

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(p => p.TenPhim.Contains(searchString));
            }

            ViewData["SearchString"] = searchString; 

            return View(movies.ToList());
        }
        #endregion

        #region thêm phim mới 
        public IActionResult Create()
        {
            var nextId = GeneratePhimId();
            ViewData["NextId"] = nextId;
            ViewBag.TheLoaiList = _context.TheLoais.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Phim phim, IFormFile apPhich)
        {
            if (ModelState.IsValid)
            {
                // Handle the file upload
                if (apPhich != null && apPhich.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image/ApPhich", apPhich.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await apPhich.CopyToAsync(stream);
                    }

                    phim.ApPhich = apPhich.FileName; // Save the filename or path to your database
                }
                if (phim.SelectedTheLoaiIds != null && phim.SelectedTheLoaiIds.Count > 0)
                {
                    phim.IdTheLoais = _context.TheLoais
                                       .Where(t => phim.SelectedTheLoaiIds.Contains(t.IdTheLoai))
                                       .ToList();
                }
                // Save the movie to the database
                _context.Add(phim);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewBag.TheLoaiList = _context.TheLoais.ToList();
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

        #endregion

        #region xóa phim
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
        #endregion

        #region chi tiết phim
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
        #endregion

        #region edit phim 
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
        public async Task<IActionResult> Edit(string id, Phim phim, IFormFile apPhich)
        {
            if (id != phim.IdPhim)
            {
                TempData["ErrorMessage"] = "ID phim không hợp lệ!";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle the file upload
                    if (apPhich != null && apPhich.Length > 0)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image/ApPhich", apPhich.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await apPhich.CopyToAsync(stream);
                        }

                        phim.ApPhich = apPhich.FileName; // Save the filename or path to your database
                    }

                    _context.Update(phim);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Phim đã được cập nhật thành công!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật phim: {ex.Message}";
                }

                return RedirectToAction(nameof(Index));
            }

            return View(phim);
        }
        #endregion
    }
}