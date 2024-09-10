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
        private readonly QlrapPhimContext _context;
        public HomeController(ILogger<HomeController> logger, QlrapPhimContext context)
        {
            _logger = logger;
            _context = context;
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
        [Route("Home/Detail/{id}")]
        public IActionResult Detail(string id)
        {
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
                    .Include(p => p.DanhGias) // Ensure the related data is included
                    .FirstOrDefault(p => p.IdPhim == id);

                if (phim == null)
                {
                    return NotFound(); // Handle case where movie is not found
                }

                // Calculate average rating or set default value of 5
                var averageRating = phim.DanhGias.Any() ? phim.DanhGias.Average(d => d.Diem) : 5;
                ViewBag.AverageRating = averageRating;

                return View(phim);
            }
        }




        [HttpGet]
        public IActionResult GetShowtimes(string cinemaId)
        {
            try
            {
                var showtimes = _context.LichChieus
                    .Where(lc => lc.IdRap == cinemaId)
                    .Select(lc => new
                    {
                        lc.IdLichChieu,
                        lc.GioChieu,
                        lc.TenPhong
                    })
                    .ToList();

                return Json(showtimes);
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                return Json(new { success = false, message = "Đã xảy ra lỗi khi lấy dữ liệu." });
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



        public IActionResult ChonGhe()
        {
            var movieId = HttpContext.Session.GetString("MovieId");
            var selectedLichChieuId = HttpContext.Session.GetString("SelectedLichChieuId");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(movieId) || string.IsNullOrEmpty(selectedLichChieuId) || string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            using (var db = new QlrapPhimContext())
            {
                var movie = db.Phims.FirstOrDefault(p => p.IdPhim == movieId);
                var lichChieu = db.LichChieus
                    .Include(l => l.IdPhongChieuNavigation)
                    .Include(l => l.IdRapNavigation)
                    .FirstOrDefault(l => l.IdLichChieu == selectedLichChieuId);

                if (movie == null || lichChieu == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Lấy danh sách chỗ ngồi đã ngồi

                var takenSeats = db.Ves
                    .Where(ticket => ticket.IdLichChieu == selectedLichChieuId && ticket.TrangThai == 1)
                    .Select(ticket => ticket.MaGheNgoi)
                    .ToList();
                // Save cinema and room names into session
                HttpContext.Session.SetString("TenRap", lichChieu.IdRapNavigation.TenRap); // Cinema Name
                HttpContext.Session.SetString("TenPhong", lichChieu.IdPhongChieuNavigation.TenPhong); // Room Name

                ViewBag.MovieId = movieId;
                ViewBag.SelectedLichChieuId = selectedLichChieuId;
                ViewBag.UserId = userId;
                ViewBag.MovieName = movie.TenPhim;
                ViewBag.TicketPrice = movie.GiaVe;
                ViewBag.GioChieu = lichChieu.GioChieu;
                ViewBag.TenPhong = lichChieu.IdPhongChieuNavigation.TenPhong;
                ViewBag.TenRap = lichChieu.IdRapNavigation.TenRap;
                ViewBag.TakenSeats = takenSeats;

                return View();
            }
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

            // Save the room and cinema names into session
            if (model.TenRap != null)
            {
                HttpContext.Session.SetString("TenRap", model.TenRap);     // Save cinema name
            }   // Save cinema name
            if (model.TenPhong != null)
            {
                HttpContext.Session.SetString("TenPhong", model.TenPhong); // Save room name
            }
            return Json(new { success = true });
        }


        // Model to receive data from the client
        public class PaymentDetailsModel
        {
            public List<string> SeatIds { get; set; }
            public int TotalAmount { get; set; }
            public string TenRap { get;  set; }
            public string TenPhong { get; set; }
        }


        public IActionResult Payment()
        {
            var seatIds = HttpContext.Session.GetString("SelectedSeatIds");
            var totalAmount = HttpContext.Session.GetInt32("TotalAmount");
            var movieId = HttpContext.Session.GetString("MovieId");
            var userId = HttpContext.Session.GetString("UserId");
            var selectedLichChieuId = HttpContext.Session.GetString("SelectedLichChieuId");
            var tenRap = HttpContext.Session.GetString("TenRap");
            var tenPhong = HttpContext.Session.GetString("TenPhong");

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
                    TrangThai = 1,
                    LoaiVe = 0,
                    IdLichChieu = selectedLichChieuId,
                    TienBanVe = totalAmount,
                    MaGheNgoi = seatIds,
                    NgayMua = DateTime.Now
                };

                // Add the ticket to the database
                db.Ves.Add(newTicket);
                db.SaveChanges();
                // Calculate loyalty points (1% of total amount)
                int loyaltyPoints = (int)(totalAmount.Value * 0.001);

                // Find the customer
                var customer = db.KhachHangs.FirstOrDefault(k => k.IdKhachHang == userId);
                if (customer != null)
                {
                    // Update the loyalty points
                    customer.DienTichLuy = (customer.DienTichLuy ?? 0) + loyaltyPoints;

                    // Save the changes to the customer
                    db.SaveChanges();
                }
                // Save the new ticket ID to the session
                HttpContext.Session.SetInt32("TicketId", newTicket.IdVe);

                // Fetch additional details for display
                var movie = db.Phims.FirstOrDefault(p => p.IdPhim == movieId);
                var lichChieu = db.LichChieus
                    .Include(l => l.IdRapNavigation)
                    .FirstOrDefault(l => l.IdLichChieu == selectedLichChieuId);
               

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
                ViewBag.TrangThai = 1;
                ViewBag.TenRap = tenRap;
                ViewBag.TenPhong = tenPhong;

            }
            return View();
        }


        public IActionResult Huyve()
        {
            UpdateTicketStatus();
            // Retrieve the user ID from the session
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home"); // Redirect if UserId is not found in session
            }

            using (var db = new QlrapPhimContext())
            {
                // Fetch the user's tickets from the database with status = 1
                var tickets = db.Ves
                    .Include(v => v.IdLichChieuNavigation)
                    .ThenInclude(lc => lc.IdPhongChieuNavigation)
                    .Include(v => v.IdLichChieuNavigation.IdPhimNavigation)
                    // Include the cinema name
                    .Include(v => v.IdLichChieuNavigation.IdRapNavigation)
                    .Include(v => v.IdKhachHangNavigation)
                    .Where(v => v.IdKhachHang == userId && v.TrangThai == 1)
                    .ToList();

                // If no tickets found, redirect to another view or show a message (optional)
                

                // Pass the tickets to the view
                return View(tickets);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelTicket([FromBody] CancelTicketRequest request)
        {
            
            using (var db = new QlrapPhimContext())
            {
                try
                {
                    // Lấy IdUser từ session
                    var userId = HttpContext.Session.GetString("UserId");

                    if (string.IsNullOrEmpty(userId))
                    {
                        return RedirectToAction("Index", "Home"); // Nếu không tìm thấy UserId, chuyển hướng về trang chủ
                    }

                    // Tìm khách hàng dựa trên IdUser
                    var customer = db.KhachHangs.FirstOrDefault(kh => kh.IdKhachHang == userId);

                    if (customer == null)
                    {
                        return NotFound("Không tìm thấy khách hàng.");
                    }

                    if (request == null || request.IdVe <= 0)
                    {
                        return BadRequest("Yêu cầu không hợp lệ.");
                    }

                    // Tìm vé dựa trên IdVe và IdKhachHang
                    var ticket = db.Ves.FirstOrDefault(v => v.IdVe == request.IdVe && v.IdKhachHang == customer.IdKhachHang);

                    if (ticket == null)
                    {
                        return NotFound("Không tìm thấy vé.");
                    }

                    if (ticket.TrangThai == 0)
                    {
                        return BadRequest("Vé đã bị hủy trước đó.");
                    }

                    // Đánh dấu vé là đã hủy (TrangThai = 0)
                    ticket.TrangThai = 0;
                    int totalAmount = (int)ticket.TienBanVe.Value;
                    int loyaltyPointsToDeduct = (int)(totalAmount * 0.001);

                    // Tính toán điểm tích lũy cần trừ


                    // Trừ điểm tích lũy của khách hàng
                    customer.DienTichLuy -= loyaltyPointsToDeduct;

                    // Đảm bảo điểm không bị âm
                    if (customer.DienTichLuy < 0)
                    {
                        customer.DienTichLuy = 0;
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();

                    return Ok("Vé đã được hủy thành công và điểm tích lũy đã được trừ.");
                }
                catch (Exception ex)
                {
                    // Ghi lại log lỗi nếu cần
                    return StatusCode(500, "Lỗi máy chủ nội bộ: " + ex.Message);
                }
            }
        }

        public IActionResult VeDaHuy()
        {
            // Retrieve the user ID from the session
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home"); // Redirect if UserId is not found in session
            }

            using (var db = new QlrapPhimContext())
            {
                // Fetch the user's tickets from the database with status = 1
                var tickets = db.Ves
                    .Include(v => v.IdLichChieuNavigation)
                    .ThenInclude(lc => lc.IdPhongChieuNavigation)
                    .Include(v => v.IdLichChieuNavigation.IdPhimNavigation)
                    .Include(v => v.IdLichChieuNavigation.IdRapNavigation)
                    .Include(v => v.IdKhachHangNavigation)
                    .Where(v => v.IdKhachHang == userId && v.TrangThai == 0)
                    .ToList();


                // Pass the tickets to the view
                return View(tickets);
            }
        }

        public IActionResult VeDaXem()
        {
            // Retrieve the user ID from the session
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home"); // Redirect if UserId is not found in session
            }

            using (var db = new QlrapPhimContext())
            {
                // Fetch the user's tickets from the database with status = 1
                var tickets = db.Ves
                    .Include(v => v.IdLichChieuNavigation)
                    .ThenInclude(lc => lc.IdPhongChieuNavigation)
                    .Include(v => v.IdLichChieuNavigation.IdPhimNavigation)
                    .Include(v => v.IdLichChieuNavigation.IdRapNavigation)
                    .Include(v => v.IdKhachHangNavigation)
                    .Where(v => v.IdKhachHang == userId && v.TrangThai == 2)
                    .ToList();

                

                // Pass the tickets to the view
                return View(tickets);
            }
        }
        

        public class CancelTicketRequest
        {
            public int IdVe { get; set; }
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
        public IActionResult TakenSeat()
        {
            var selectedLichChieuId = HttpContext.Session.GetString("SelectedLichChieuId");
            using (var db = new QlrapPhimContext())
            {
                // Lấy tất cả các giá trị trong cột MaGheNgoi từ cơ sở dữ liệu
                var seatStrings = db.Ves
                    .Where(v => v.IdLichChieu == selectedLichChieuId)
                    .Select(v => v.MaGheNgoi)
                    .ToList();

                // Tạo một tập hợp để lưu ghế đã đặt
                var bookedSeats = new HashSet<string>();

                // Tách chuỗi và thêm các ghế vào tập hợp
                foreach (var seatString in seatStrings)
                {
                    if (!string.IsNullOrEmpty(seatString))
                    {
                        var seats = seatString.Split(',');
                        foreach (var seat in seats)
                        {
                            bookedSeats.Add(seat.Trim());
                        }
                    }
                }

                // Trả về danh sách ghế đã đặt dưới dạng JSON
                return Json(bookedSeats.ToList());
            }


        }
        [Route("FAQ")]
        public IActionResult FAQ()
        {
            return View();
        }
        public void UpdateTicketStatus()
        {
            using (var db = new QlrapPhimContext())
            {
                var now = DateTime.Now;

                // Get all tickets with valid status (assuming 1 is valid)
                var tickets = db.Ves
                    .Include(v => v.IdLichChieuNavigation)
                    .Where(v => v.TrangThai == 1) // Only consider tickets that are currently valid
                    .ToList();

                foreach (var ticket in tickets)
                {
                    if (ticket.IdLichChieuNavigation.GioChieu < now)
                    {
                        // If the showtime has passed, mark the ticket as invalid (assuming 0 is invalid)
                        ticket.TrangThai = 2;
                    }
                }

                // Save changes to the database
                db.SaveChanges();
            }
        }

        #region MoviesShowing

        public IActionResult MoviesShowing(string searchString, string selectedGenre)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var moviesQuery = _context.Phims
                .Include(p => p.IdTheLoais)
                .Where(p => p.NgayKhoiChieu <= today && p.NgayKetThuc >= today);

            if (!string.IsNullOrEmpty(searchString))
            {
                moviesQuery = moviesQuery.Where(p => p.TenPhim.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(selectedGenre))
            {
                moviesQuery = moviesQuery.Where(p => p.IdTheLoais.Any(tl => tl.TenTheLoai == selectedGenre));
            }

            ViewData["SearchString"] = searchString;
            ViewData["SelectedGenre"] = selectedGenre;

            // Fetch unique genres for the filter dropdown
            ViewData["Genres"] = _context.TheLoais.Select(tl => tl.TenTheLoai).Distinct().ToList();

            return View(moviesQuery.ToList());
        }
        #endregion

        #region MovieComingSoon
        public IActionResult MoviesComingSoon(string searchString, string selectedGenre)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var moviesQuery = _context.Phims
                .Include(p => p.IdTheLoais)
                .Where(p => p.NgayKhoiChieu > today);

            if (!string.IsNullOrEmpty(searchString))
            {
                moviesQuery = moviesQuery.Where(p => p.TenPhim.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(selectedGenre))
            {
                moviesQuery = moviesQuery.Where(p => p.IdTheLoais.Any(tl => tl.TenTheLoai == selectedGenre));
            }

            ViewData["SearchString"] = searchString;
            ViewData["SelectedGenre"] = selectedGenre;

            // Fetch unique genres for the filter dropdown
            ViewData["Genres"] = _context.TheLoais.Select(tl => tl.TenTheLoai).Distinct().ToList();

            return View(moviesQuery.ToList());
        }

        #endregion

        public IActionResult HoanVe(int idVe)
        {
            using (var _context = new QlrapPhimContext())
            {
                var ticket = _context.Ves
                    .Include(v => v.IdLichChieuNavigation)
                    .ThenInclude(lc => lc.IdPhimNavigation)
                    .Include(v => v.IdLichChieuNavigation.IdRapNavigation)
                    .Include(v => v.IdLichChieuNavigation.IdPhongChieuNavigation)
                    .Include(v => v.IdKhachHangNavigation)
                    .FirstOrDefault(v => v.IdVe == idVe);

                if (ticket == null)
                {
                    return NotFound("Vé không tồn tại.");
                }
                
                return View("HoanVe", ticket); // Gọi view HoanVe
            }
        }
        [HttpPost]
        public IActionResult ProcessRefund(int IdVe, int refundReason, string customReason = null)
        {
            var result = new { Success = false, Message = string.Empty };

            try
            {
                using (var _context = new QlrapPhimContext())
                {
                    // Retrieve the ticket from the database
                    var ticket = _context.Ves.Find(IdVe);
                    if (ticket == null)
                    {
                        result = new { Success = false, Message = "Ticket not found." };
                        return Json(result);
                    }

                    // Update the ticket status to 'Refunded' or a suitable status
                    ticket.TrangThai = refundReason; // Save the refund reason

                    // Save custom reason if applicable
                    if (refundReason == 6 && !string.IsNullOrEmpty(customReason)) // Assuming 6 is for custom reason
                    {
                        ticket.NoiDung = customReason;
                    }
                    else
                    {
                        // Use predefined reasons
                        ticket.NoiDung = refundReason switch
                        {
                            3 => "Đặt nhầm vé",
                            4 => "Có việc bận không thể xem phim",
                            5 => "Sự cố giao thông hoặc vấn đề di chuyển",
                            _ => ticket.NoiDung
                        };
                    }

                    _context.SaveChanges();

                    result = new { Success = true, Message = "Refund processed successfully." };
                   
                }
            }
            catch (Exception ex)
            {
                result = new { Success = false, Message = $"Error processing refund: {ex.Message}" };
            }

            return Json(result);
            
        }

        public IActionResult ViewRefundedTickets()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home"); // Redirect if UserId is not found in session
            }

            using (var db = new QlrapPhimContext())
            {
                // Lấy khách hàng dựa vào userId từ session
                var customer = db.KhachHangs.FirstOrDefault(kh => kh. IdKhachHang== userId);

                if (customer == null)
                {
                    return NotFound("Không tìm thấy khách hàng.");
                }

                // Lấy các vé có trạng thái 3, 4, hoặc 5 từ cơ sở dữ liệu
                var tickets = db.Ves
                    .Include(v => v.IdLichChieuNavigation)
                    .ThenInclude(lc => lc.IdPhongChieuNavigation)
                    .Include(v => v.IdLichChieuNavigation.IdPhimNavigation)
                    .Include(v => v.IdLichChieuNavigation.IdRapNavigation)
                    .Include(v => v.IdKhachHangNavigation)
                    .Where(v => v.IdKhachHang == customer.IdKhachHang && (v.TrangThai == 3 || v.TrangThai == 4 || v.TrangThai == 5))
                    .ToList();

                // Trừ điểm tích lũy dựa vào số vé đã hoàn
               
                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();


                // Trả về view với danh sách vé đã hoàn
                return View(tickets);
            }
        }
        #region review 
        // GET: Home/CreateReview
        public IActionResult CreateReview(string idPhim)
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "AccountKH");
            }

            // Gán IdPhim vào ViewBag
            ViewBag.IdPhim = idPhim;
            ViewBag.UserName = userName;
            ViewBag.UserId = userId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview([Bind("IdDanhGia,IdPhim,NoiDung,Diem,TrangThaiDanhGia")] DanhGia danhGia)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để đánh giá." });
            }

            if (string.IsNullOrEmpty(danhGia.IdPhim))
            {
                return Json(new { success = false, message = "ID phim không hợp lệ." });
            }

            danhGia.NgayDanhGia = DateTime.Now;
            danhGia.IdKhachHang = userId;

            var existingReview = await _context.DanhGias
                .FirstOrDefaultAsync(r => r.IdPhim == danhGia.IdPhim && r.IdKhachHang == userId);

            if (existingReview != null)
            {
                return Json(new { success = false, message = "Bạn đã đánh giá phim này rồi." });
            }

            _context.Add(danhGia);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đánh giá của bạn đã được gửi thành công." });
        }

        #endregion

    }
}

