using FinalProject_3K1D.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject_3K1D.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ScheduleManagementController : Controller
    {
        private readonly QlrapPhimContext _context;

        public ScheduleManagementController(QlrapPhimContext context)
        {
            _context = context;
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

        // GET: /Admin/ScheduleManagement/Create
        public IActionResult Create()
        {
            ViewData["idPhim"] = new SelectList(_context.Phims, "IdPhim", "TenPhim");
            ViewData["idRap"] = new SelectList(_context.Raps, "IdRap", "TenRap");
            ViewData["NextId"] = GenerateNewIdLichChieu(); // Set the next ID here
            return View();
        }

        // POST: /Admin/ScheduleManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GioChieu,IdPhongChieu,GiaVe,IdPhim,IdRap")] LichChieu lichChieu)
        {
            if (ModelState.IsValid)
            {
                lichChieu.IdLichChieu = GenerateNewIdLichChieu(); // Set new ID
                _context.Add(lichChieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPhim"] = new SelectList(_context.Phims, "IdPhim", "TenPhim", lichChieu.IdPhim);
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap", lichChieu.IdRap);
            return View(lichChieu);
        }

        // GET: /Admin/ScheduleManagement/GetPhongChieuByRap
        [HttpGet]
        public JsonResult GetPhongChieuByRap(string idRap)
        {
            var phongChieus = _context.PhongChieus
                .Where(pc => pc.IdRap == idRap)
                .Select(pc => new { pc.IdPhongChieu, pc.TenPhong })
                .ToList();

            return Json(phongChieus);
        }

        // Method to generate a new ID for LichChieu
        private string GenerateNewIdLichChieu()
        {
            // Get all existing IDs from the database
            var existingIds = _context.LichChieus
                .Select(l => l.IdLichChieu)
                .ToList();

            // If no IDs exist, start with "LC001"
            if (!existingIds.Any())
            {
                return "LC001";
            }

            // Find the highest number from existing IDs
            int maxNumber = existingIds
                .Select(id =>
                {
                    // Extract number part from ID
                    var numberPart = id.Substring(2); // Remove prefix "LC"
                    return int.TryParse(numberPart, out int result) ? result : 0;
                })
                .Max();

            // Create new ID by incrementing the highest number
            var newNumber = maxNumber + 1;

            // Ensure new ID has the correct format with prefix and zero-padding
            return "LC" + newNumber.ToString("D3"); // "D3" ensures 3-digit format
        }
    }
}
