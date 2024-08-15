using Microsoft.AspNetCore.Mvc;
using FinalProject_3K1D.Models;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject_3K1D.Controllers
{
    public class GiamGiaController : Controller
    {
        private readonly QlrapPhimContext _context;

        public GiamGiaController(QlrapPhimContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var discounts = _context.KhuyenMais.ToList();
            return View(discounts);
        }

        public IActionResult Claim(int id)
        {
            var discount = _context.KhuyenMais.FirstOrDefault(d => d.IdKhuyenMai == id);
            if (discount == null)
            {
                return NotFound();
            }

            // Implement the logic for claiming the discount here

            // For demonstration, we'll just redirect to the index view
            return RedirectToAction("Index");
        }
    }
}

