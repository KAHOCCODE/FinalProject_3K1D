using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
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
        [Route("Admin/Employees/Create")]
        public IActionResult Create()
        {
            var newEmployee = new NhanVien
            {
                IdNhanVien = GenerateEmployeeId() // Generate ID here
            };

            ViewData["IdChucVu"] = new SelectList(_context.ChucVus, "IdChucVu", "TenChucVu");
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap");
            ViewData["NextId"] = newEmployee.IdNhanVien;

            return View(newEmployee);
        }

        [Route("Admin/Employees/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdNhanVien,HoTen,NgaySinh,DiaChi,Sdt,Email,UserNv,PassNv,IdChucVu,IdRap")] NhanVien nhanVien, IFormFile HinhAnhFile)
        {
            // Check if the model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if an image file is provided
                    if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                    {
                        // Ensure the "AnhNhanVien" directory exists
                        var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image/AnhNhanVien");
                        if (!Directory.Exists(imagesDirectory))
                        {
                            Directory.CreateDirectory(imagesDirectory);
                        }

                        // Generate a unique filename
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(HinhAnhFile.FileName)}";
                        var filePath = Path.Combine(imagesDirectory, fileName);

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await HinhAnhFile.CopyToAsync(stream);
                        }

                        // Update the NhanVien model with the image filename
                        nhanVien.HinhAnh = fileName;
                    }

                    // Add the new employee to the database
                    _context.Add(nhanVien);
                    await _context.SaveChangesAsync();

                    // Redirect to the Index action
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Handle the error (you might want to log it and show a user-friendly message)
                    ModelState.AddModelError("", "An error occurred while creating the employee. Please try again.");
                }
            }

            // If we got this far, something went wrong; redisplay the form
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
                        // Ensure the "AnhNhanVien" directory exists
                        var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image/AnhNhanVien");
                        if (!Directory.Exists(imagesDirectory))
                        {
                            Directory.CreateDirectory(imagesDirectory);
                        }

                        // Generate a unique filename to avoid conflicts
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(HinhAnhFile.FileName)}";
                        var filePath = Path.Combine(imagesDirectory, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await HinhAnhFile.CopyToAsync(stream);
                        }

                        nhanVien.HinhAnh = fileName; // Update the HinhAnh property
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

        private string GenerateEmployeeId()
        {
            var lastEmployee = _context.NhanViens
                .OrderByDescending(e => e.IdNhanVien)
                .FirstOrDefault();

            if (lastEmployee != null)
            {
                int nextIdNumber = int.Parse(lastEmployee.IdNhanVien.Substring(2)) + 1;
                string nextId = $"NV{nextIdNumber:D4}";

                while (_context.NhanViens.Any(e => e.IdNhanVien == nextId))
                {
                    nextIdNumber++;
                    nextId = $"NV{nextIdNumber:D4}";
                }

                return nextId;
            }

            return "NV0001";
        }
    }
}
