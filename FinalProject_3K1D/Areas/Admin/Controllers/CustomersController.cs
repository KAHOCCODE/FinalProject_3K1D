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
    public class CustomersController : Controller
    {
        private readonly QlrapPhimContext _context;

        public CustomersController(QlrapPhimContext context)
        {
            _context = context;
        }
        [Route("Admin/Customers")]
        // GET: Admin/Customers
        public async Task<IActionResult> Index()
        {
            return View(await _context.KhachHangs.ToListAsync());
        }
        [Route("Admin/Customers/Details/{id}")]
        // GET: Admin/Customers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.IdKhachHang == id);
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
            var nextId = GenerateKhachHangId();
            ViewData["NextId"] = nextId;
            return View();
        }

        [Route("Admin/Customers/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HoTen,NgaySinh,DiaChi,Sdt,Cccd,Email,UserKh,PassKh")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    khachHang.IdKhachHang = GenerateKhachHangId();
                    // Add the new customer to the database
                    _context.KhachHangs.Add(khachHang);
                    await _context.SaveChangesAsync();

                    // Store success message in TempData
                    TempData["SuccessMessage"] = "Khách hàng đã được thêm thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log the exception details here
                    Console.WriteLine($"Error creating customer: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the customer.");
                }
            }
            return View(khachHang);
        }


        [Route("Admin/Customers/Edit/{id}")]
        // GET: Admin/Customers/Edit/5
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

        // POST: Admin/Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        // GET: Admin/Customers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.IdKhachHang == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }
        [Route("Admin/Customers/Delete/{id}")]
        // POST: Admin/Customers/Delete/5
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

        private string GenerateKhachHangId()
        {
            // Logic to generate the next IdKhachHang based on the last IdKhachHang in the database
            var lastCustomer = _context.KhachHangs.OrderByDescending(c => c.IdKhachHang).FirstOrDefault();
            if (lastCustomer != null)
            {
                int nextId = int.Parse(lastCustomer.IdKhachHang.Substring(2)) + 1;
                return $"KH{nextId:D3}";
            }
            return "KH01";
        }
    }
}
