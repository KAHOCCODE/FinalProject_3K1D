using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProject_3K1D.Models;
using System.IO;

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

        [Route("Admin/Employees")]
        public async Task<IActionResult> Index()
        {
            var employees = _context.NhanViens.Include(n => n.IdChucVuNavigation).Include(n => n.IdRapNavigation);
            return View(await employees.ToListAsync());
        }

        [Route("Admin/Employees/Details/{id}")]
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
        public async Task<IActionResult> Create([Bind("IdNhanVien,HoTen,NgaySinh,DiaChi,Sdt,HinhAnh,Email,UserNv,PassNv,IdChucVu,IdRap")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(nhanVien.IdNhanVien))
                    {
                        nhanVien.IdNhanVien = GenerateEmployeeId();
                    }

                    // Save the uploaded image
                    if (Request.Form.Files.Count > 0)
                    {
                        var file = Request.Form.Files[0];
                        if (file.Length > 0 && (file.ContentType == "image/jpeg" || file.ContentType == "image/png"))
                        {
                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            nhanVien.HinhAnh = fileName;
                        }
                    }

                    _context.NhanViens.Add(nhanVien);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Employee created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View(nhanVien);
                }
            }
            ViewData["IdChucVu"] = new SelectList(_context.ChucVus, "IdChucVu", "TenChucVu", nhanVien.IdChucVu);
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap", nhanVien.IdRap);
            return View(nhanVien);
        }
        [Route("Admin/Employees/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            ViewData["IdChucVu"] = new SelectList(_context.ChucVus, "IdChucVu", "TenChucVu", nhanVien.IdChucVu);
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap", nhanVien.IdRap);
            return View(nhanVien);
        }

        [Route("Admin/Employees/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdNhanVien,HoTen,NgaySinh,DiaChi,Sdt,Email,UserNv,PassNv,IdRap,IdChucVu")] NhanVien nhanVien, IFormFile newImage)
        {
            if (id != nhanVien.IdNhanVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEmployee = await _context.NhanViens.FindAsync(id);

                    if (existingEmployee == null)
                    {
                        return NotFound();
                    }

                    // Update only specific fields
                    existingEmployee.HoTen = nhanVien.HoTen;
                    existingEmployee.NgaySinh = nhanVien.NgaySinh;
                    existingEmployee.DiaChi = nhanVien.DiaChi;
                    existingEmployee.Sdt = nhanVien.Sdt;
                    existingEmployee.Email = nhanVien.Email;
                    existingEmployee.UserNv = nhanVien.UserNv;
                    existingEmployee.PassNv = nhanVien.PassNv;
                    existingEmployee.IdRap = nhanVien.IdRap;
                    existingEmployee.IdChucVu = nhanVien.IdChucVu;

                    // Handle image upload
                    if (newImage != null && newImage.Length > 0 && (newImage.ContentType == "image/jpeg" || newImage.ContentType == "image/png"))
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(newImage.FileName)}";
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await newImage.CopyToAsync(stream);
                        }

                        // Update image filename
                        existingEmployee.HinhAnh = fileName;
                    }

                    _context.Update(existingEmployee);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Employee updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = nhanVien.IdNhanVien });
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
            }

            ViewData["IdChucVu"] = new SelectList(_context.ChucVus, "IdChucVu", "TenChucVu", nhanVien.IdChucVu);
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap", nhanVien.IdRap);
            return View(nhanVien);
        }


        [Route("Admin/Employees/Delete/{id}")]
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

        [Route("Admin/Employees/Delete/{id}")]
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
