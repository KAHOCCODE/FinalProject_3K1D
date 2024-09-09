using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FinalProject_3K1D.Models;

namespace _3K1D_Final.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeAdminController : Controller
    {
        private readonly QlrapPhimContext _context;

        public HomeAdminController(QlrapPhimContext context)
        {
            _context = context;
        }

        [Route("Admin/HomeAdmin")]
        [Route("Admin/")]
        [Route("Index")]
        public IActionResult Index()
        {
            var totalCustomers = _context.KhachHangs.Count();
            var totalEmployees = _context.NhanViens.Count();
            var totalMovies = _context.Phims.Count();

            // Doanh thu theo tên rạp
            var revenueByTheater = _context.Ves
                .Join(_context.LichChieus,
                      v => v.IdLichChieu,
                      lc => lc.IdLichChieu,
                      (v, lc) => new { v, lc })
                .Join(_context.Raps,
                      lc => lc.lc.IdRap,
                      r => r.IdRap,
                      (lc, r) => new { lc.v, lc.lc, r })
                .GroupBy(x => x.r.TenRap)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.v.TienBanVe ?? 0)
                );

            // Top 3 phim có doanh thu cao nhất theo từng rạp
            var topMoviesByTheater = _context.Ves
                .Join(_context.LichChieus,
                      v => v.IdLichChieu,
                      lc => lc.IdLichChieu,
                      (v, lc) => new { v, lc })
                .Join(_context.Raps,
                      lc => lc.lc.IdRap,
                      r => r.IdRap,
                      (lc, r) => new { lc.v, lc.lc, r })
                .GroupBy(x => new { x.r.TenRap, x.lc.IdPhim })
                .Select(g => new
                {
                    TheaterName = g.Key.TenRap,
                    MovieTitle = _context.Phims.FirstOrDefault(p => p.IdPhim == g.Key.IdPhim).TenPhim,
                    TotalRevenue = g.Sum(x => x.v.TienBanVe ?? 0)
                })
                .GroupBy(x => x.TheaterName)
                .Select(g => new DashboardViewModel.TheaterTopMovies
                {
                    TheaterName = g.Key,
                    TopMovies = g.OrderByDescending(x => x.TotalRevenue)
                                 .Take(3)
                                 .Select(x => new DashboardViewModel.MovieRevenue
                                 {
                                     MovieTitle = x.MovieTitle,
                                     TotalRevenue = x.TotalRevenue
                                 })
                                 .ToList()
                })
                .ToList();

            var model = new DashboardViewModel
            {
                TotalCustomers = totalCustomers,
                TotalEmployees = totalEmployees,
                TotalMovies = totalMovies,
                RevenueByTheater = revenueByTheater,
                TopMoviesByTheater = topMoviesByTheater
            };

            return View(model);
        }
    }
}
