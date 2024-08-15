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
        [Route("Admin/Employees")]
        public async Task<IActionResult> Index()
        {
            var qlrapPhimContext = _context.NhanViens.Include(n => n.IdChucVuNavigation).Include(n => n.IdRapNavigation);
            return View(await qlrapPhimContext.ToListAsync());
        }
        [Route("Admin/Employees/Details/{id}")]

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
        [Route("Admin/Employees/Create")]

        // GET: Admin/Employees/Create
        public IActionResult Create()
        {
            ViewData["IdChucVu"] = new SelectList(_context.ChucVus, "IdChucVu", "TenChucVu");
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "TenRap");
            var nextId = GenerateEmployeeId();
            ViewData["NextId"] = nextId;
            return View();
        }

        // POST: Admin/Employees/Create
        [Route("Admin/Employees/Create")]
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
            ViewData["IdChucVu"] = new SelectList(await _context.ChucVus.ToListAsync(), "Id", "Name");
            ViewData["IdRap"] = new SelectList(await _context.Raps.ToListAsync(), "Id", "Name");
            return View(nhanVien);
        }

        [Route("Admin/Employees/Edit/{id}")]
        // GET: Admin/Employees/Edit/5
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
            ViewData["IdChucVu"] = new SelectList(_context.ChucVus, "IdChucVu", "IdChucVu", nhanVien.IdChucVu);
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "IdRap", nhanVien.IdRap);
            return View(nhanVien);
        }

        // POST: Admin/Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Admin/Employees/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdNhanVien,HoTen,NgaySinh,DiaChi,Sdt,HinhAnh,Email,UserNv,PassNv,IdRap,IdChucVu")] NhanVien nhanVien)
        {
            if (id != nhanVien.IdNhanVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["IdChucVu"] = new SelectList(_context.ChucVus, "IdChucVu", "IdChucVu", nhanVien.IdChucVu);
            ViewData["IdRap"] = new SelectList(_context.Raps, "IdRap", "IdRap", nhanVien.IdRap);
            return View(nhanVien);
        }

        // GET: Admin/Employees/Delete/5
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
            // Logic to generate the next IdNhanVien based on the last IdNhanVien in the database
            var lastEmployee = _context.NhanViens.OrderByDescending(e => e.IdNhanVien).FirstOrDefault();
            if (lastEmployee != null)
            {
                int nextId = int.Parse(lastEmployee.IdNhanVien.Substring(2)) + 1;
                return $"NV{nextId:D3}";
            }
            return "NV001";
        }
    }
}
