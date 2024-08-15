using FinalProject_3K1D.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace FinalProject_3K1D.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RevenueByTimesController : Controller
    {
        
        private readonly QlrapPhimContext _context;

        public RevenueByTimesController(QlrapPhimContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Ves.AsQueryable();

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
                    TotalRevenue = g.Sum(v => v.TienBanVe) ?? 0
                })
                .OrderBy(d => d.Date)
                .ToList();

            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;

            return View(data);
        }

        public IActionResult ExportToExcel(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Ves.AsQueryable();

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
                    TotalRevenue = g.Sum(v => v.TienBanVe) ?? 0
                })
                .OrderBy(d => d.Date)
                .ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Doanh Thu");
                worksheet.Cells[1, 1].Value = "Ngày";
                worksheet.Cells[1, 2].Value = "Tổng Doanh Thu";

                var row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 2].Value = item.TotalRevenue;
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"Doanh_Thu_{DateTime.Now.ToString("yyyyMMdd")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
