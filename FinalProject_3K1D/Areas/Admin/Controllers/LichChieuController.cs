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
    public class LichChieuController : Controller
    {
        private readonly QlrapPhimContext _context;

        public LichChieuController(QlrapPhimContext context)
        {
            _context = context;
        }
        

        // GET: Admin/LichChieux
        public async Task<IActionResult> Index()
        {
            ViewBag.PhongChieus = _context.PhongChieus.ToList();
            ViewBag.Phims = _context.Phims.ToList();
            ViewBag.LichChieus = _context.LichChieus.ToList();

            return View();
        }
        

        // Trong Controller
       
        // GET: Admin/LichChieux/Details/5
        public async Task<IActionResult> Details(string id)
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

        // GET: Admin/LichChieux/Create
        public IActionResult Create()
        {
            ViewBag.PhongChieus = _context.PhongChieus.ToList();
            ViewBag.Phims = _context.Phims.ToList();
            ViewBag.LichChieus = _context.LichChieus.ToList();
            ViewData["IdPhim"] = new SelectList(_context.Phims, "IdPhim", "IdPhim");
            ViewData["IdPhongChieu"] = new SelectList(_context.PhongChieus, "IdPhongChieu", "IdPhongChieu");
            var id = Request.Query["id"].ToString();
            using (var db = new QlrapPhimContext())
            {
                var phim = db.Phims
                    .Include(p => p.IdTheLoais)
                    .FirstOrDefault(p => p.IdPhim == id);
                return View(phim);
            }
            return View();
        }

        // POST: Admin/LichChieux/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdLichChieu,GioChieu,IdPhongChieu,GiaVe,TrangThai,IdPhim")] LichChieu lichChieu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lichChieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPhim"] = new SelectList(_context.Phims, "IdPhim", "IdPhim", lichChieu.IdPhim);
            ViewData["IdPhongChieu"] = new SelectList(_context.PhongChieus, "IdPhongChieu", "IdPhongChieu", lichChieu.IdPhongChieu);
            return View(lichChieu);
        }

        // GET: Admin/LichChieux/Edit/5
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
            ViewData["IdPhim"] = new SelectList(_context.Phims, "IdPhim", "IdPhim", lichChieu.IdPhim);
            ViewData["IdPhongChieu"] = new SelectList(_context.PhongChieus, "IdPhongChieu", "IdPhongChieu", lichChieu.IdPhongChieu);
            return View(lichChieu);
        }

        // POST: Admin/LichChieux/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdLichChieu,GioChieu,IdPhongChieu,GiaVe,TrangThai,IdPhim")] LichChieu lichChieu)
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
            ViewData["IdPhim"] = new SelectList(_context.Phims, "IdPhim", "IdPhim", lichChieu.IdPhim);
            ViewData["IdPhongChieu"] = new SelectList(_context.PhongChieus, "IdPhongChieu", "IdPhongChieu", lichChieu.IdPhongChieu);
            return View(lichChieu);
        }

        // GET: Admin/LichChieux/Delete/5
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

        // POST: Admin/LichChieux/Delete/5
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
