using FinalProject_3K1D.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FinalProject_3K1D.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ScheduleManagementController : Controller
    {
        private readonly QlrapPhimContext _context;
        private readonly ILogger<ScheduleManagementController> _logger;

        // Constructor with ILogger injected
        public ScheduleManagementController(QlrapPhimContext context, ILogger<ScheduleManagementController> logger)
        {
            _context = context;
            _logger = logger; // Initialize the logger
        }

        // GET: /Admin/ScheduleManagement
        public async Task<IActionResult> Index()
        {
            var lichChieus = await _context.LichChieus
                .Include(l => l.IdPhimNavigation)
                .Include(l => l.IdPhongChieuNavigation)
                .Include(p => p.IdPhongChieuNavigation.IdRapNavigation)
                .ToListAsync();

            return View(lichChieus);
        }

        [Route("Admin/ScheduleManagement/Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichChieu = await _context.LichChieus
                .Include(l => l.IdPhimNavigation)
                .Include(l => l.IdPhongChieuNavigation)
                .Include(l => l.IdPhongChieuNavigation.IdRapNavigation)
                .FirstOrDefaultAsync(m => m.IdLichChieu == id);

            if (lichChieu == null)
            {
                return NotFound();
            }

            return View(lichChieu);
        }

        // GET: /Admin/ScheduleManagement/Create
        [Route("Admin/ScheduleManagement/Create")]
        public IActionResult Create()
        {
            var newLichChieu = new LichChieu
            {
                IdLichChieu = GenerateNewIdLichChieu()
            };

            ViewData["NextId"] = newLichChieu.IdLichChieu;
            ViewData["IdPhim"] = new SelectList(_context.Phims, "IdPhim", "TenPhim");
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap");
            return View(newLichChieu);
        }

        [Route("Admin/ScheduleManagement/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdLichChieu,GioChieu,IdPhongChieu,GiaVe,IdPhim,IdRap")] LichChieu lichChieu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(lichChieu.IdLichChieu))
                    {
                        lichChieu.IdLichChieu = GenerateNewIdLichChieu();
                    }

                    _context.LichChieus.Add(lichChieu);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Lịch chiếu đã được thêm thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while creating LichChieu");
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm lịch chiếu.";
                    return View(lichChieu);
                }
            }

            ViewData["IdPhim"] = new SelectList(_context.Phims, "IdPhim", "TenPhim", lichChieu.IdPhim);
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap", lichChieu.IdRap);
            return View(lichChieu);
        }


        [HttpGet]
        public JsonResult GetPhongChieuByRap(string idRap)
        {
            var phongChieus = _context.PhongChieus
                .Where(pc => pc.IdRap == idRap)
                .Select(pc => new { pc.IdPhongChieu, pc.TenPhong })
                .ToList();

            return Json(phongChieus);
        }

        private string GenerateNewIdLichChieu()
        {
            var existingIds = _context.LichChieus
                .Select(l => l.IdLichChieu)
                .ToList();

            if (!existingIds.Any())
            {
                return "LC001";
            }

            int maxNumber = existingIds
                .Select(id =>
                {
                    var numberPart = id.Substring(2);
                    return int.TryParse(numberPart, out int result) ? result : 0;
                })
                .Max();

            var newNumber = maxNumber + 1;
            return "LC" + newNumber.ToString("D3");
        }

        #region edit
        [Route("Admin/ScheduleManagement/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichChieu = await _context.LichChieus.FindAsync(id);
            if (lichChieu == null)
            {
                return NotFound();
            }
            ViewData["IdPhim"] = new SelectList(_context.Phims, "IdPhim", "TenPhim", lichChieu.IdPhim);
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap", lichChieu.IdRap);

            return View(lichChieu);
        }

        [Route("Admin/ScheduleManagement/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdLichChieu,GioChieu,IdPhongChieu,GiaVe,IdPhim,IdRap")] LichChieu lichChieu)
        {
            if (id != lichChieu.IdLichChieu)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichChieu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichChieuExists(lichChieu.IdLichChieu))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(lichChieu);
        }
        #endregion

        #region delete
        [Route("Admin/ScheduleManagement/Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichChieu = await _context.LichChieus
                .Include(l => l.IdPhimNavigation)
                .Include(l => l.IdPhongChieuNavigation)
                .FirstOrDefaultAsync(m => m.IdLichChieu == id);

            if (lichChieu == null)
            {
                return NotFound();
            }

            return View(lichChieu);
        }

        [Route("Admin/ScheduleManagement/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var lichChieu = await _context.LichChieus.FindAsync(id);
            if (lichChieu != null)
            {
                _context.LichChieus.Remove(lichChieu);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LichChieuExists(string id)
        {
            return _context.LichChieus.Any(e => e.IdLichChieu == id);
        }
        #endregion
    }
}
