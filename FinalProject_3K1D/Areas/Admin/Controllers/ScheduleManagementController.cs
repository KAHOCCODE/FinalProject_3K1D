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

        [Route("Admin/ScheduleManagement/Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichChieu = await _context.LichChieus
                .Include(l => l.IdPhimNavigation) // Include related data from the Phim table
                .Include(l => l.IdPhongChieuNavigation) // Include related data from the PhongChieu table
                .Include(l => l.IdPhongChieuNavigation.IdRapNavigation) // Include related data from the Rap table
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
            // Tạo một đối tượng LichChieu mới và gán ID mới
            var newLichChieu = new LichChieu
            {
                IdLichChieu = GenerateNewIdLichChieu() // Tạo ID ở đây
            };

            // Đặt ID này vào ViewData để sử dụng trong View
            ViewData["NextId"] = newLichChieu.IdLichChieu;

            ViewData["idPhim"] = new SelectList(_context.Phims, "IdPhim", "TenPhim");
            ViewData["idRap"] = new SelectList(_context.Raps, "IdRap", "TenRap");
            return View(newLichChieu);
        }


       
        // POST: /Admin/ScheduleManagement/Create
        [Route("Admin/ScheduleManagement/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdLichChieu,GioChieu,IdPhongChieu,GiaVe,IdPhim,IdRap")] LichChieu lichChieu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra lại xem lichChieu.IdLichChieu đã có giá trị chưa
                    if (string.IsNullOrEmpty(lichChieu.IdLichChieu))
                    {
                        // Nếu chưa, tạo một mã mới
                        lichChieu.IdLichChieu = GenerateNewIdLichChieu();
                    }

                    // Thêm lịch chiếu mới vào cơ sở dữ liệu
                    _context.LichChieus.Add(lichChieu);
                    await _context.SaveChangesAsync();

                    // Lưu thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Lịch chiếu đã được thêm thành công!";
                    return RedirectToAction("Index");
                }
                catch 
                {
                    // Xử lý lỗi tại đây (có thể ghi log)
                    
                    return View(lichChieu);
                }
            }

            // Nếu ModelState không hợp lệ, trả về view cùng với dữ liệu dropdown
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

       
        // GET: /Admin/ScheduleManagement/Delete/{id}

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
                   
            }

                await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool LichChieuExists(string id)
        {
            return _context.LichChieus.Any(e => e.IdLichChieu == id);
        }


    }
}
