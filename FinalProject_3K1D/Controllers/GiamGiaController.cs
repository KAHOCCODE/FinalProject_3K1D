using ZXing;
using ZXing.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FinalProject_3K1D.Models;
using Microsoft.AspNetCore.Mvc;
using ZXing.QrCode;

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

       
    }
}
