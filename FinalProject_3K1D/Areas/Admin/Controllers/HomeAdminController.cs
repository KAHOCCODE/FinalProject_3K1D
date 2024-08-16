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

            var revenueByTheater = _context.Ves
                .Join(_context.LichChieus,
                      v => v.IdLichChieu,
                      lc => lc.IdLichChieu,
                      (v, lc) => new { v, lc })
                .GroupBy(x => x.lc.IdRap)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.v.TienBanVe ?? 0)
                );

            var model = new DashboardViewModel
            {
                TotalCustomers = totalCustomers,
                TotalEmployees = totalEmployees,
                TotalMovies = totalMovies,
                RevenueByTheater = revenueByTheater
            };

            return View(model);
        }
    }
}
