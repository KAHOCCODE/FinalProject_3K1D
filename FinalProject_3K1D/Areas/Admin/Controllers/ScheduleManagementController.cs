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

        #region Index
        public async Task<IActionResult> Index()
        {
            var lichChieus = await _context.LichChieus
                .Include(l => l.IdPhimNavigation)
                .Include(l => l.IdPhongChieuNavigation)
                .Include(p => p.IdPhongChieuNavigation.IdRapNavigation)
                .ToListAsync();

            return View(lichChieus);
        }
        #endregion

        #region Details
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
        #endregion

        #region create
        [Route("Admin/ScheduleManagement/Create")]
        public IActionResult Create()
        {
            var newLichChieu = new LichChieu
            {
                IdLichChieu = GenerateNewIdLichChieu()
            };
            SetViewDataForSelectLists();
            ViewData["NextId"] = newLichChieu.IdLichChieu;
            return View(newLichChieu);
        }

        [Route("Admin/ScheduleManagement/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdLichChieu,GioChieu,IdPhongChieu,GiaVe,IdPhim,IdRap")] LichChieu lichChieu)
        {
            lichChieu.IdLichChieu = GenerateNewIdLichChieu();

            // Tìm kiếm Rạp và phòng chiếu để thêm vào Navigation Property
            lichChieu.IdRapNavigation = await _context.Raps.FirstOrDefaultAsync(r => r.IdRap == lichChieu.IdRap);
            lichChieu.IdPhongChieuNavigation = await _context.PhongChieus.FirstOrDefaultAsync(pc => pc.IdPhongChieu == lichChieu.IdPhongChieu);

            _context.LichChieus.Add(lichChieu);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Lịch chiếu đã được thêm thành công!";
            return RedirectToAction("Index");

            SetViewDataForSelectLists(lichChieu);
            return View(lichChieu);
        }

        private void SetViewDataForSelectLists(LichChieu lichChieu = null)
        {
            ViewData["IdPhim"] = new SelectList(_context.Phims, "IdPhim", "TenPhim", lichChieu?.IdPhim);
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap", lichChieu?.IdRap);
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
                return "LC01";
            }

            int maxNumber = existingIds
                .Select(id =>
                {
                    var numberPart = id.Substring(2);
                    return int.TryParse(numberPart, out int result) ? result : 0;
                })
                .Max();

            var newNumber = maxNumber + 1;
            return "LC" + newNumber.ToString("D2");
        }
        #endregion

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
