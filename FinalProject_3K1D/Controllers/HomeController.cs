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
            var movieId = HttpContext.Session.GetString("MovieId");
            var userId = HttpContext.Session.GetString("UserId");
            var selectedLichChieuId = HttpContext.Session.GetString("SelectedLichChieuId");

            if (string.IsNullOrEmpty(seatIds) || totalAmount == null || string.IsNullOrEmpty(movieId) || string.IsNullOrEmpty(userId))
            {
                // If data is missing, display an error message
                ViewBag.ErrorMessage = "Dữ liệu không hợp lệ. Vui lòng chọn lại.";
                return View("ChonGhe"); // Redirect to the seat selection page
            }

            using (var db = new QlrapPhimContext())
            {
                // Generate a new ticket ID
                int newTicketId = GenerateTicketId();

                // Create a new ticket object
                var newTicket = new Ve
                {
                    IdVe = newTicketId,
                    IdKhachHang = userId,
                    TrangThai = 0,
                    LoaiVe = 0,
                    IdLichChieu = selectedLichChieuId,
                    TienBanVe = totalAmount,
                    MaGheNgoi = seatIds,
                    NgayMua = DateTime.Now
                };

                // Add the ticket to the database
                db.Ves.Add(newTicket);
                db.SaveChanges();

                // Save the new ticket ID to the session
                HttpContext.Session.SetInt32("TicketId", newTicket.IdVe);

                // Fetch additional details for display
                var movie = db.Phims.FirstOrDefault(p => p.IdPhim == movieId);
                var lichChieu = db.LichChieus
                    .Include(l => l.IdRapNavigation)
                    .FirstOrDefault(l => l.IdLichChieu == selectedLichChieuId);
                var customer = db.KhachHangs.FirstOrDefault(u => u.IdKhachHang == userId);

                if (movie == null || lichChieu == null || customer == null)
                {
                    return RedirectToAction("Index", "Home"); // Redirect if data not found
                }

                // Pass data to the ViewBag
                ViewBag.MovieName = movie.TenPhim;
                ViewBag.CustomerName = customer.HoTen;
                ViewBag.GioChieu = lichChieu.GioChieu;
                ViewBag.SelectedSeatIds = seatIds;
                ViewBag.TotalAmount = totalAmount;
                ViewBag.TicketId = newTicketId; // Pass the generated ticket ID to the view
                ViewBag.CustomerId = userId;
                ViewBag.LoaiVe = 0;
                
                ViewBag.TrangThai = 0;

            }
            return View();
        }


        public IActionResult Huyve()
        {
            // Retrieve the user ID from the session
            var userId = HttpContext.Session.GetString("UserId");
            
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home"); // Redirect if UserId is not found in session
            }

            using (var db = new QlrapPhimContext())
            {
                // Fetch the user's tickets from the database
                var tickets = db.Ves
                    .Include(v => v.IdLichChieuNavigation)
                    .ThenInclude(lc => lc.IdPhongChieuNavigation)
                    .Include(v => v.IdLichChieuNavigation.IdPhimNavigation)
                    .Include(v => v.IdKhachHangNavigation)
                    .Where(v => v.IdKhachHang == userId)
                    .ToList();

                // If no tickets found, redirect to another view or show a message (optional)
                if (!tickets.Any())
                {
                    return RedirectToAction("Index", "Home"); // Or you can return a view with a message
                }

                // Pass the tickets to the view (for demonstration, we'll pass the first ticket)
                var ticket = tickets.FirstOrDefault();

                if (ticket != null)
                {
                    ViewBag.MovieTitle = ticket.IdLichChieuNavigation?.IdPhimNavigation?.TenPhim ?? "Unknown";
                    ViewBag.Showtime = ticket.IdLichChieuNavigation?.GioChieu.ToString() ?? "Unknown";
                    ViewBag.Room = ticket.IdLichChieuNavigation?.IdPhongChieuNavigation?.TenPhong ?? "Unknown";
                    ViewBag.CustomerName = ticket.IdKhachHangNavigation?.HoTen ?? "Unknown";
                    ViewBag.Price = ticket.TienBanVe.HasValue ? ticket.TienBanVe.Value.ToString() : "Unknown";
                    ViewBag.IdVe = ticket.IdVe;
                    ViewBag.IdSeat = ticket.MaGheNgoi;
                }

                return View(ticket);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelTicket(int idVe)
        {
            using (var _context = new QlrapPhimContext())
            {
                try
                {
                    var ticket = _context.Ves.Find(idVe);

                    if (ticket == null)
                    {
                        return BadRequest("Ticket not found.");
                    }

                    _context.Ves.Remove(ticket);
                    _context.SaveChanges();

                    return Ok();
                }
                catch (Exception ex)
                {
                    // Log the exception (e.g., using a logging framework)
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }
        }

        private int GenerateTicketId()
        {
            using (var db = new QlrapPhimContext())
            {
                // Get the highest existing ticket ID
                var lastTicket = db.Ves
                    .OrderByDescending(v => v.IdVe)
                    .FirstOrDefault();

                if (lastTicket != null)
                {
                    // Increment the last ID by 1
                    return lastTicket.IdVe + 1;
                }
                return 1; // Start with ID 1 if no tickets exist
            }
        }




    }
}

