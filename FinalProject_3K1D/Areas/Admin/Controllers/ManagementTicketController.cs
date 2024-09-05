using Microsoft.AspNetCore.Mvc;
using FinalProject_3K1D.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_3K1D.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManagementTicketController : Controller
    {
        private readonly QlrapPhimContext _context;

        public ManagementTicketController(QlrapPhimContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            // Fetch all ticket data with related entities
            var tickets = _context.Ves
                 .Where(v => v.TrangThai == 1)
                .Select(v => new
                {
                    v.IdVe,
                    TenPhim = v.IdLichChieuNavigation.IdPhimNavigation.TenPhim,
                    HoTen = v.IdKhachHangNavigation.HoTen,
                    v.MaGheNgoi,
                    v.TienBanVe,
                    v.NgayMua,
                    GioChieu = v.IdLichChieuNavigation.GioChieu,
                    TenPhong = v.IdLichChieuNavigation.IdPhongChieuNavigation.TenPhong
                });

            // Filter tickets by movie name if a search string is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                tickets = tickets.Where(t => t.TenPhim.Contains(searchString));
            }

            // Pass the data to the view
            return View(tickets.ToList());
        }
        public IActionResult HuyVe(string searchString1)
        {
            // Fetch all ticket data with related entities
            var tickets = _context.Ves
                 .Where(v => v.TrangThai == 0)
                .Select(v => new
                {
                    v.IdVe,
                    TenPhim = v.IdLichChieuNavigation.IdPhimNavigation.TenPhim,
                    HoTen = v.IdKhachHangNavigation.HoTen,
                    v.MaGheNgoi,
                    v.TienBanVe,
                    v.NgayMua,
                    GioChieu = v.IdLichChieuNavigation.GioChieu,
                    TenPhong = v.IdLichChieuNavigation.IdPhongChieuNavigation.TenPhong
                });

            // Filter tickets by movie name if a search string is provided
            if (!string.IsNullOrEmpty(searchString1))
            {
                tickets = tickets.Where(t => t.TenPhim.Contains(searchString1));
            }

            // Pass the data to the view
            return View(tickets.ToList());
        }
        public IActionResult VeDaXem(string searchString2)
        {
            // Fetch all ticket data with related entities
            var tickets = _context.Ves
                 .Where(v => v.TrangThai == 2)
                .Select(v => new
                {
                    v.IdVe,
                    TenPhim = v.IdLichChieuNavigation.IdPhimNavigation.TenPhim,
                    HoTen = v.IdKhachHangNavigation.HoTen,
                    v.MaGheNgoi,
                    v.TienBanVe,
                    v.NgayMua,
                    GioChieu = v.IdLichChieuNavigation.GioChieu,
                    TenPhong = v.IdLichChieuNavigation.IdPhongChieuNavigation.TenPhong
                });

            // Filter tickets by movie name if a search string is provided
            if (!string.IsNullOrEmpty(searchString2))
            {
                tickets = tickets.Where(t => t.TenPhim.Contains(searchString2));
            }

            // Pass the data to the view
            return View(tickets.ToList());
        }
        public IActionResult HoanVe(string searchString3)
        {

            var tickets = _context.Ves
            .Where(v => v.TrangThai == 3 || v.TrangThai == 4 || v.TrangThai == 5 || v.TrangThai == 6)
           .Select(v => new
           {
               v.IdVe,
               TenPhim = v.IdLichChieuNavigation.IdPhimNavigation.TenPhim,
               HoTen = v.IdKhachHangNavigation.HoTen,
               v.MaGheNgoi,
               v.TienBanVe,
               v.NgayMua,
               GioChieu = v.IdLichChieuNavigation.GioChieu,
               TenPhong = v.IdLichChieuNavigation.IdPhongChieuNavigation.TenPhong,
               NoiDung = v.NoiDung
           });

            // Filter tickets by movie name if a search string is provided
            if (!string.IsNullOrEmpty(searchString3))
            {
                tickets = tickets.Where(t => t.TenPhim.Contains(searchString3));
            }

            // Pass the data to the view
            return View(tickets.ToList());
        }
        [HttpPost]
        public IActionResult XLHoanVe(int idVe)
        {
            var ticket = _context.Ves.FirstOrDefault(v => v.IdVe == idVe);
            if (ticket == null) return NotFound();

            // Update ticket status
            ticket.TrangThai = 7;

            // Calculate accumulated points
            var points = ticket.TienBanVe * 0.001m;

            // Get current time and show time difference
            var now = DateTime.Now;
            var showTime = ticket.GioChieu;
            var hoursDifference = (now - showTime).TotalHours;

            // Get refund percentage from GiaHoan table
            var refundPercentage = _context.GiaHoans
                .Where(g => g.ThoiGianHoan <= hoursDifference)
                .OrderByDescending(g => g.ThoiGianHoan)
                .Select(g => g.PhanTramHoan)
                .FirstOrDefault();

            // Calculate refund amount
            var refundAmount = (ticket.TienBanVe ?? 0) * (decimal)(refundPercentage);

            // Update ticket price
            ticket.TienBanVe = refundAmount;

            // Save changes
            _context.SaveChanges();

            return Json(new { success = true });
        }


    }
}
