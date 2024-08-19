using FinalProject_3K1D.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FinalProject_3K1D.Controllers
{
    //sử dụng session (paste code này vào)
//    var userName = HttpContext.Session.GetString("UserName");
//    var userId = HttpContext.Session.GetString("UserId");
//    var userRole = HttpContext.Session.GetString("UserRole");

//    if (string.IsNullOrEmpty(userName))
//    {
//        return RedirectToAction("Login", "AccountKH");
//}

//ViewBag.UserName = userName;
//ViewBag.UserRole = userRole;
 // ví dụ xem ở detail
    
public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            using (var db = new QlrapPhimContext())
            {
                var phims = db.Phims
                    .Include(p => p.IdTheLoais)
                    .ToList();
                return View(phims);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Detail()
        {
            var id = Request.Query["id"].ToString();
            if (string.IsNullOrEmpty(id))
            {
                return NotFound(); // Handle case where id is not provided
            }

            using (var db = new QlrapPhimContext())
            {
                var phim = db.Phims
                    .Include(p => p.IdTheLoais)
                    .Include(p => p.LichChieus)
                        .ThenInclude(l => l.IdPhongChieuNavigation)
                    .Include(p => p.LichChieus)
                        .ThenInclude(l => l.IdRapNavigation)
                    .FirstOrDefault(p => p.IdPhim == id);

                if (phim == null)
                {
                    return NotFound(); // Handle case where movie is not found
                }

                return View(phim);
            }
        }

        public IActionResult _Home()
        {
            using (var db = new QlrapPhimContext())
            {
                var phims = db.Phims
                    .Include(p => p.IdTheLoais)
                    .ToList();
                return View(phims);
            }
        }

        [HttpPost]
        public IActionResult SaveBookingSession([FromBody] BookingSessionModel model)
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userId))
            {
                // If the user is not logged in, redirect to the login page
                return Json(new { success = false, message = "Bạn cần đăng nhập trước khi đặt vé." });
            }

            // Save the UserId to the BookingSessionModel
            model.UserId = userId;

            // Save the movie ID into the session
            HttpContext.Session.SetString("MovieId", model.MovieId);

            // Save selected LichChieuId into the session
            HttpContext.Session.SetString("SelectedLichChieuId", model.SelectedLichChieuId);

            return Json(new { success = true });
        }



        // Model để nhận dữ liệu từ client
        public IActionResult ChonGhe()
        {
            // Retrieve data from session
            var movieId = HttpContext.Session.GetString("MovieId");
            var selectedLichChieuId = HttpContext.Session.GetString("SelectedLichChieuId");
            var userId = HttpContext.Session.GetString("UserId"); // Retrieve UserId from session

            if (string.IsNullOrEmpty(movieId) || string.IsNullOrEmpty(selectedLichChieuId) || string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home"); // Redirect if necessary information is missing from session
            }

            // Fetch movie details using movieId and selectedLichChieuId
            using (var db = new QlrapPhimContext())
            {
                var movie = db.Phims.FirstOrDefault(p => p.IdPhim == movieId);
                var lichChieu = db.LichChieus
                    .Include(l => l.IdPhongChieuNavigation)
                    .Include(l => l.IdRapNavigation)
                    .FirstOrDefault(l => l.IdLichChieu == selectedLichChieuId);

                if (movie == null || lichChieu == null)
                {
                    return RedirectToAction("Index", "Home"); // Redirect if movie or schedule not found
                }

                // Pass movie and schedule details to the view
                ViewBag.MovieId = movieId;
                ViewBag.SelectedLichChieuId = selectedLichChieuId;
                ViewBag.UserId = userId;
                ViewBag.MovieName = movie.TenPhim; // Pass TenPhim (movie name) to the view
                ViewBag.TicketPrice = movie.GiaVe; // Ticket price

                // Additional information from LichChieu
                ViewBag.GioChieu = lichChieu.GioChieu; // Thời gian chiếu
                ViewBag.TenPhong = lichChieu.IdPhongChieuNavigation.TenPhong; // Tên phòng
            }

            return View();
        }


        [HttpPost]
        public IActionResult SavePaymentDetails([FromBody] PaymentDetailsModel model)
        {
            if (model.SeatIds == null || model.TotalAmount <= 0)
            {
                return Json(new { success = false, message = "Dữ liệu ghế hoặc tổng cộng không hợp lệ." });
            }

            // Save the seat IDs and total amount into session
            HttpContext.Session.SetString("SelectedSeatIds", string.Join(",", model.SeatIds));
            HttpContext.Session.SetInt32("TotalAmount", model.TotalAmount);

            return Json(new { success = true });
        }

        // Model to receive data from the client
        public class PaymentDetailsModel
        {
            public List<string> SeatIds { get; set; }
            public int TotalAmount { get; set; }
        }


        public IActionResult Payment()
        {
            var seatIds = HttpContext.Session.GetString("SelectedSeatIds");
            var totalAmount = HttpContext.Session.GetInt32("TotalAmount");

            if (string.IsNullOrEmpty(seatIds) || totalAmount == null)
            {
                // If data is missing, show an error message
                ViewBag.ErrorMessage = "Dữ liệu ghế hoặc tổng cộng không hợp lệ. Vui lòng chọn lại ghế.";
                return View("ChonGhe"); // Redirect back to seat selection
            }

            // Pass the seat IDs and total amount to the view for payment processing
            ViewBag.SelectedSeatIds = seatIds;
            ViewBag.TotalAmount = totalAmount;

            return View();
        }


    }
}
