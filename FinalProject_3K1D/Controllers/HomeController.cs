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
            //var userName = HttpContext.Session.GetString("UserName");
            //var userId = HttpContext.Session.GetString("UserId");
            //var userRole = HttpContext.Session.GetString("UserRole");

            //if (string.IsNullOrEmpty(userName))
            //{
            //    return RedirectToAction("Login", "AccountKH");
            //}

            //ViewBag.UserName = userName;
            //ViewBag.UserRole = userRole;
            var id = Request.Query["id"].ToString();
            using (var db = new QlrapPhimContext())
            {
                var phim = db.Phims
                    .Include(p => p.IdTheLoais)
                    .Include(p => p.LichChieus)
                    .FirstOrDefault(p => p.IdPhim == id);
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

            // Save selected times into the session as a JSON string
            HttpContext.Session.SetString("SelectedTimes", Newtonsoft.Json.JsonConvert.SerializeObject(model.SelectedTimes));

            // Optionally, save selected seats as well
            if (model.SelectedSeats != null)
            {
                HttpContext.Session.SetString("SelectedSeats", Newtonsoft.Json.JsonConvert.SerializeObject(model.SelectedSeats));
            }

            return Json(new { success = true });
        }


        // Model để nhận dữ liệu từ client
        public IActionResult ChonGhe()
        {
            // Retrieve data from session
            var movieId = HttpContext.Session.GetString("MovieId");
            var selectedTimesJson = HttpContext.Session.GetString("SelectedTimes");
            var userId = HttpContext.Session.GetString("UserId"); // Retrieve UserId from session

            if (string.IsNullOrEmpty(movieId) || string.IsNullOrEmpty(selectedTimesJson) || string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home"); // Redirect if necessary information is missing from session
            }

            var selectedTimes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(selectedTimesJson);

            // Use movieId, selectedTimes, and userId in your logic
            ViewBag.MovieId = movieId;
            ViewBag.SelectedTimes = selectedTimes;
            ViewBag.UserId = userId; // Pass UserId to the view

            return View();
        }

        public IActionResult Payment()
        {
            var movieId = HttpContext.Session.GetString("MovieId");
            var selectedTimesJson = HttpContext.Session.GetString("SelectedTimes");
            var selectedSeatsJson = HttpContext.Session.GetString("SelectedSeats");

            if (string.IsNullOrEmpty(movieId) || string.IsNullOrEmpty(selectedTimesJson) || string.IsNullOrEmpty(selectedSeatsJson))
            {
                return RedirectToAction("Index", "Home"); // Chuyển hướng nếu thông tin không tồn tại trong session
            }

            var selectedTimes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(selectedTimesJson);
            var selectedSeats = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(selectedSeatsJson);

            ViewBag.MovieId = movieId;
            ViewBag.SelectedTimes = selectedTimes;
            ViewBag.SelectedSeats = selectedSeats;

            return View();
        }

    }
}
