using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalProject_3K1D.Models;

namespace FinalProject_3K1D.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomersController : Controller
    {
        private readonly QlrapPhimContext _context;

        public CustomersController(QlrapPhimContext context)
        {
            _context = context;
        }

        [Route("Admin/Customers")]
        public async Task<IActionResult> Index(string searchString)
        {
            var customers = _context.KhachHangs.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(c => c.HoTen.Contains(searchString) ||
                                                 c.Email.Contains(searchString) ||
                                                 c.Sdt.Contains(searchString));
            }

            ViewData["SearchString"] = searchString;

            return View(await customers.ToListAsync());
        }

        [Route("Admin/Customers/Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(m => m.IdKhachHang == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        [Route("Admin/Customers/Create")]
        // GET: Admin/Customers/Create
        public IActionResult Create()
        {
            var newCustomer = new KhachHang
            {
                IdKhachHang = GenerateCustomerId() // Generate ID here
            };

            // Đặt ID này vào ViewData để sử dụng trong View
            ViewData["NextId"] = newCustomer.IdKhachHang;

            return View(newCustomer);
        }

        [Route("Admin/Customers/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdKhachHang,HoTen,NgaySinh,DiaChi,Sdt,Cccd,Email,UserKh,PassKh")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra lại xem khachHang.IdKhachHang đã có giá trị chưa
                    if (string.IsNullOrEmpty(khachHang.IdKhachHang))
                    {
                        // Nếu chưa, tạo một mã mới
                        khachHang.IdKhachHang = GenerateCustomerId();
                    }

                    _context.KhachHangs.Add(khachHang);
                    await _context.SaveChangesAsync();

                    // Lưu thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Khách hàng đã được thêm thành công!";
                    return RedirectToAction("Index");
                }
                catch
                {
                    // Xử lý lỗi tại đây
                    return View(khachHang);
                }
            }
            return View(khachHang);
        }

        [Route("Admin/Customers/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return View(khachHang);
        }

        [Route("Admin/Customers/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdKhachHang,HoTen,NgaySinh,DiaChi,Sdt,Cccd,DienTichLuy,Email,UserKh,PassKh")] KhachHang khachHang)
        {
            if (id != khachHang.IdKhachHang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khachHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangExists(khachHang.IdKhachHang))
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
            return View(khachHang);
        }

        [Route("Admin/Customers/Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(m => m.IdKhachHang == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        [Route("Admin/Customers/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang != null)
            {
                _context.KhachHangs.Remove(khachHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachHangExists(string id)
        {
            return _context.KhachHangs.Any(e => e.IdKhachHang == id);
        }

        private string GenerateCustomerId()
        {
            // Tìm ID khách hàng cuối cùng trong cơ sở dữ liệu
            var lastCustomer = _context.KhachHangs
                .OrderByDescending(k => k.IdKhachHang)
                .FirstOrDefault();

            if (lastCustomer != null)
            {
                // Tăng giá trị ID cuối cùng lên 1
                int nextIdNumber = int.Parse(lastCustomer.IdKhachHang.Substring(2)) + 1;
                string nextId = $"KH{nextIdNumber:D4}";

                // Đảm bảo ID là duy nhất
                while (_context.KhachHangs.Any(k => k.IdKhachHang == nextId))
                {
                    nextIdNumber++;
                    nextId = $"KH{nextIdNumber:D4}";
                }

                return nextId;
            }
            return "KH0001"; // ID bắt đầu nếu chưa có khách hàng nào trong cơ sở dữ liệu
        }

    }
}

