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
        // POST: /Admin/ScheduleManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GioChieu,IdPhongChieu,GiaVe,IdPhim,IdRap")] LichChieu lichChieu)
        {
            if (ModelState.IsValid)
            {
                // Generate a new ID for the LichChieu record
                lichChieu.IdLichChieu = GenerateNewIdLichChieu();

                // Add the new record to the database context
                _context.Add(lichChieu);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Redirect to the Index action after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If model state is not valid, reload the dropdown lists and return the view
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
        // GET: /Admin/ScheduleManagement/Edit/{id}
        public async Task<IActionResult> Edit(string id)
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

            ViewData["idPhim"] = new SelectList(_context.Phims, "IdPhim", "TenPhim", lichChieu.IdPhim);
            ViewData["idRap"] = new SelectList(_context.Raps, "IdRap", "TenRap", lichChieu.IdRap);
            return View(lichChieu);
        }

        // POST: /Admin/ScheduleManagement/Edit/{id}
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
            ViewData["idPhim"] = new SelectList(_context.Phims, "IdPhim", "TenPhim", lichChieu.IdPhim);
            ViewData["idRap"] = new SelectList(_context.Raps, "IdRap", "TenRap", lichChieu.IdRap);
            return View(lichChieu);
        }

        private bool LichChieuExists(string id)
        {
            return _context.LichChieus.Any(e => e.IdLichChieu == id);
        }

    }
}
