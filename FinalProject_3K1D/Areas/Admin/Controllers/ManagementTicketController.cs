using Microsoft.AspNetCore.Mvc;
using FinalProject_3K1D.Models;
using System.Linq;

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
    }
}
