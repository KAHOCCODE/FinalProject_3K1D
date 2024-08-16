using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProject_3K1D.Models;

namespace FinalProject_3K1D.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeesController : Controller
    {
        private readonly QlrapPhimContext _context;

        public EmployeesController(QlrapPhimContext context)
        {
            _context = context;
        }

        // GET: Admin/Employees
        public async Task<IActionResult> Index()
        {
            var qlrapPhimContext = _context.NhanViens.Include(n => n.IdChucVuNavigation).Include(n => n.IdRapNavigation);
            return View(await qlrapPhimContext.ToListAsync());
        }

        // GET: Admin/Employees/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .Include(n => n.IdChucVuNavigation)
                .Include(n => n.IdRapNavigation)
                .FirstOrDefaultAsync(m => m.IdNhanVien == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }
        // GET: Admin/Employees/Create
        public IActionResult Create()
        {
            // Generate a new employee ID
            var lastEmployee = _context.NhanViens.OrderByDescending(e => e.IdNhanVien).FirstOrDefault();
            string newId = GenerateNewEmployeeId(lastEmployee?.IdNhanVien);

            // Pass the new ID to the view
            ViewBag.NewEmployeeId = newId;

            ViewData["IdChucVu"] = new SelectList(_context.ChucVus, "IdChucVu", "TenChucVu"); // Display role names
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap"); // Display theater names
            return View();
        }

        private string GenerateNewEmployeeId(string lastId)
        {
            if (lastId == null)
            {
                return "NV001"; // Starting ID
            }

            // Extract numeric part and increment
            string prefix = "NV";
            int number = int.Parse(lastId.Substring(prefix.Length)) + 1;
            return prefix + number.ToString("D3"); // Format with leading zeros
        }


        // POST: Admin/Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdNhanVien,HoTen,NgaySinh,DiaChi,Sdt,HinhAnh,Email,UserNv,PassNv,IdRap,IdChucVu")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdChucVu"] = new SelectList(_context.ChucVus, "IdChucVu", "TenChucVu", nhanVien.IdChucVu);
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap", nhanVien.IdRap);
            return View(nhanVien);
        }

        // GET: Admin/Employees/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .Include(n => n.IdChucVuNavigation)
                .Include(n => n.IdRapNavigation)
                .FirstOrDefaultAsync(m => m.IdNhanVien == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            ViewData["IdChucVu"] = new SelectList(_context.ChucVus, "IdChucVu", "TenChucVu", nhanVien.IdChucVu);
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap", nhanVien.IdRap);
            return View(nhanVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdNhanVien,HoTen,NgaySinh,DiaChi,Sdt,HinhAnh,Email,UserNv,PassNv,IdRap,IdChucVu")] NhanVien nhanVien, IFormFile HinhAnhFile)
        {
            if (id != nhanVien.IdNhanVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if a new image file is uploaded
                    if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                    {
                        // Create the directory if it does not exist
                        var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "~/wwwroot/images");
                        if (!Directory.Exists(imagesDirectory))
                        {
                            Directory.CreateDirectory(imagesDirectory);
                        }

                        // Process the uploaded file
                        var fileName = Path.GetFileName(HinhAnhFile.FileName); // Ensure no path traversal
                        var filePath = Path.Combine(imagesDirectory, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await HinhAnhFile.CopyToAsync(stream);
                        }

                        nhanVien.HinhAnh = fileName; // Set the image path relative to wwwroot
                    }

                    _context.Update(nhanVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(nhanVien.IdNhanVien))
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
            ViewData["IdChucVu"] = new SelectList(_context.ChucVus, "IdChucVu", "TenChucVu", nhanVien.IdChucVu);
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap", nhanVien.IdRap);
            return View(nhanVien);
        }

        // GET: Admin/Employees/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .Include(n => n.IdChucVuNavigation)
                .Include(n => n.IdRapNavigation)
                .FirstOrDefaultAsync(m => m.IdNhanVien == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // POST: Admin/Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien != null)
            {
                _context.NhanViens.Remove(nhanVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienExists(string id)
        {
            return _context.NhanViens.Any(e => e.IdNhanVien == id);
        }
    }
}
