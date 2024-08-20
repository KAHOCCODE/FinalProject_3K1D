using Microsoft.AspNetCore.Mvc;
using FinalProject_3K1D.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_3K1D.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieRevenueController : Controller
    {
        private readonly QlrapPhimContext _context;

        public MovieRevenueController(QlrapPhimContext context)
        {
            _context = context;
        }

        #region Index
        public IActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Ves
                .Include(v => v.IdLichChieuNavigation)
                .ThenInclude(lc => lc.IdPhimNavigation)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(v => v.NgayMua >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(v => v.NgayMua <= endDate.Value);
            }

            var data = query
                .GroupBy(v => new { v.IdLichChieuNavigation.IdPhimNavigation.TenPhim })
                .Select(g => new
                {
                    MovieName = g.Key.TenPhim,
                    TotalRevenue = g.Sum(v => v.TienBanVe) ?? 0
                })
                .OrderBy(d => d.MovieName)
                .ToList();

            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;

            return View(data);
        }
        #endregion

        #region Details 
        public IActionResult Details(string movieName, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Ves
                .Include(v => v.IdLichChieuNavigation)
                .ThenInclude(lc => lc.IdPhimNavigation)
                .Where(v => v.IdLichChieuNavigation.IdPhimNavigation.TenPhim == movieName)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(v => v.NgayMua >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(v => v.NgayMua <= endDate.Value);
            }

            var data = query
                .GroupBy(v => v.NgayMua.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TicketsSold = g.Count(),
                    DailyRevenue = g.Sum(v => v.TienBanVe) ?? 0
                })
                .OrderBy(d => d.Date)
                .ToList();

            ViewData["MovieName"] = movieName;
            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;

            return View(data);
        }
        #endregion
    }
}
