using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProject_3K1D.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using Microsoft.AspNetCore.Identity;
using FinalProject_3K1D.ViewModels;
using Microsoft.CodeAnalysis.Scripting;
using System.Net.Mail;
using System.Net;
using BCrypt.Net;
using Microsoft.Extensions.Options;


namespace FinalProject_3K1D.Controllers
{
    public class AccountKHController : Controller
    {
        private readonly QlrapPhimContext _context;
        private readonly SmtpSettings _smtpSettings;

        public AccountKHController(QlrapPhimContext context, IOptions<SmtpSettings> smtpSettings)
        {
            _context = context;
            _smtpSettings = smtpSettings.Value;

        }

        #region Register
        [HttpGet]
        public IActionResult Register(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterKH model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra các điều kiện tồn tại như bạn đã làm
                var existingUser = await _context.KhachHangs.FirstOrDefaultAsync(kh => kh.UserKh == model.UserKH);
                if (existingUser != null)
                {
                    ModelState.AddModelError("UserKH", "Tên người dùng đã tồn tại.");
                    return View(model);
                }

                var existingSdt = await _context.KhachHangs.FirstOrDefaultAsync(kh => kh.Sdt == model.SDT);
                if (existingSdt != null)
                {
                    ModelState.AddModelError("SDT", "Số điện thoại đã tồn tại.");
                    return View(model);
                }

                var existingCccd = await _context.KhachHangs.FirstOrDefaultAsync(kh => kh.Cccd == model.CCCD);
                if (existingCccd != null)
                {
                    ModelState.AddModelError("CCCD", "Số chứng minh nhân dân đã tồn tại.");
                    return View(model);
                }

                var existingEmail = await _context.KhachHangs.FirstOrDefaultAsync(kh => kh.Email == model.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                    return View(model);
                }

                // Tạo IdKhachHang mới
                string newId = "KH01";
                var lastUser = await _context.KhachHangs.OrderByDescending(kh => kh.IdKhachHang).FirstOrDefaultAsync();
                if (lastUser != null)
                {
                    string lastId = lastUser.IdKhachHang;
                    if (lastId.Length > 2)
                    {
                        string lastIdPart = lastId.Substring(2);
                        if (int.TryParse(lastIdPart, out int lastIdNumber))
                        {
                            newId = $"KH{(lastIdNumber + 1):D2}";
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error generating ID.");
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error generating ID: ID format is invalid.");
                        return View(model);
                    }
                }

                // Mã hóa mật khẩu
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.PassKH);

                // Tạo người dùng mới
                var newUser = new KhachHang
                {
                    IdKhachHang = newId,
                    HoTen = model.HoTen,
                    NgaySinh = model.NgaySinh,
                    Sdt = model.SDT,
                    Cccd = model.CCCD,
                    Email = model.Email,
                    UserKh = model.UserKH,
                    PassKh = hashedPassword
                };

                try
                {
                    await _context.KhachHangs.AddAsync(newUser);
                    await _context.SaveChangesAsync();

                    // Gửi email thông báo đăng ký thành công
                    string emailMessage = $@"
            <p>Xin chào {newUser.HoTen},</p>
            <p>Cảm ơn bạn đã đăng ký tài khoản thành công tại hệ thống của chúng tôi.</p>
            <p>Thông tin tài khoản của bạn là:</p>
            <ul>
                <li>Tên đăng nhập: <strong>{newUser.UserKh}</strong></li>
                <li>Email: <strong>{newUser.Email}</strong></li>
            </ul>
            <p>Vui lòng đăng nhập để sử dụng các dịch vụ của chúng tôi.</p>
            <p>Trân trọng,</p>
            <p>Đội ngũ hỗ trợ khách hàng.</p>";

                    await SendEmail(newUser.Email, "Đăng ký tài khoản thành công", emailMessage);

                    // Thông báo thành công và chuyển hướng
                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login", "AccountKH");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu người dùng. Vui lòng thử lại.");
                    return View(model);
                }
            }

            return View(model);
        }





        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginKH model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;

            if (ModelState.IsValid)
            {
                if (model.Role == "KhachHang")
                {
                    // Tìm kiếm khách hàng bằng UserKh (username)
                    var khachhang = await _context.KhachHangs
                        .SingleOrDefaultAsync(kh => kh.UserKh == model.UserKh);

                    if (khachhang == null || !BCrypt.Net.BCrypt.Verify(model.PassKh, khachhang.PassKh))
                    {
                        ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                        return View(model);
                    }

                    // Logic lưu trữ session và claims
                    HttpContext.Session.SetString("UserId", khachhang.IdKhachHang);
                    HttpContext.Session.SetString("UserName", khachhang.HoTen);
                    HttpContext.Session.SetString("UserRole", "KhachHang");

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, khachhang.HoTen),
                new Claim(ClaimTypes.Email, khachhang.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, "KhachHang"),
                new Claim("UserId", khachhang.IdKhachHang)
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(claimsPrincipal);

                    TempData["SuccessMessage"] = "Đăng nhập thành công!";
                    return RedirectToAction("Index", "Home");
                }
                else if (model.Role == "NhanVien")
                {
                    var nhanvien = await _context.NhanViens
                        .SingleOrDefaultAsync(nv => nv.UserNv == model.UserKh);

                    if (nhanvien == null || !BCrypt.Net.BCrypt.Verify(model.PassKh, nhanvien.PassNv))
                    {
                        ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                        return View(model);
                    }

                    HttpContext.Session.SetString("UserId", nhanvien.IdNhanVien);
                    HttpContext.Session.SetString("UserName", nhanvien.HoTen);
                    HttpContext.Session.SetString("UserRole", "NhanVien");

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, nhanvien.HoTen),
                new Claim(ClaimTypes.Email, nhanvien.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, "NhanVien"),
                new Claim("UserId", nhanvien.IdNhanVien)
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(claimsPrincipal);

                    TempData["SuccessMessage"] = "Đăng nhập thành công!";
                    return Redirect("/Admin");
                }
            }
            return View(model);
        }


        #endregion

        #region logoutt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear(); // Xóa tất cả session
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Profile
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "AccountKH");
            }

            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.IdKhachHang == userId);
            if (khachHang == null)
            {
                return RedirectToAction("Login", "AccountKH");
            }

            return View(khachHang);

        }

        [HttpPost]
        public IActionResult UpdateProfile(KhachHang updatedKhachHang)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "AccountKH");
            }

            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.IdKhachHang == userId);
            if (khachHang == null)
            {
                return RedirectToAction("Login", "AccountKH");
            }

            // Cập nhật thông tin (trừ SDT và Điểm Tích Lũy)
            khachHang.HoTen = updatedKhachHang.HoTen;
            khachHang.NgaySinh = updatedKhachHang.NgaySinh;
            khachHang.Email = updatedKhachHang.Email;
            khachHang.Cccd = updatedKhachHang.Cccd;
            khachHang.DiaChi = updatedKhachHang.DiaChi;

            _context.Update(khachHang);
            _context.SaveChanges();

            return RedirectToAction("Profile");
        }

        #endregion

        #region ChangePassword
        // GET: Hiển thị form thay đổi mật khẩu
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Xử lý yêu cầu thay đổi mật khẩu
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    // Lấy IdKhachHang từ session
    var userId = HttpContext.Session.GetString("UserId");
    if (userId == null)
    {
        return RedirectToAction("Login");
    }

    // Lấy thông tin khách hàng từ cơ sở dữ liệu
    var khachhang = await _context.KhachHangs
        .SingleOrDefaultAsync(kh => kh.IdKhachHang == userId);

    if (khachhang == null)
    {
        ModelState.AddModelError("", "Người dùng không tồn tại.");
        TempData["ErrorMessage"] = "Người dùng không tồn tại.";
        return View(model);
    }

    // Kiểm tra mật khẩu hiện tại (so sánh mật khẩu băm)
    if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, khachhang.PassKh))
    {
        ModelState.AddModelError("", "Mật khẩu hiện tại không đúng.");
        TempData["ErrorMessage"] = "Mật khẩu hiện tại không đúng.";
        return View(model);
    }

    // Cập nhật mật khẩu mới (mã hóa mật khẩu)
    khachhang.PassKh = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
    _context.KhachHangs.Update(khachhang);
    await _context.SaveChangesAsync();

    TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công!";
    return RedirectToAction("ChangePassword");
}

        #endregion

        #region ForgotPassword
        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QuenMatKhau(string email)
        {
            var khachHang = await _context.KhachHangs.SingleOrDefaultAsync(kh => kh.Email == email);
            if (khachHang == null)
            {
                ViewBag.ErrorMessage = "Email không tồn tại.";
                return View();
            }

            var newPassword = GenerateRandomPassword();
            khachHang.PassKh = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            string message = $@"
        <p>Xin chào: {khachHang.HoTen},</p>
        <p>Mật khẩu mới của bạn là: <strong>{newPassword}</strong></p>
        <p>Vui lòng đăng nhập lại và đổi mật khẩu mới để đảm bảo an toàn.</p>";

            await SendEmail(khachHang.Email, "Mật khẩu mới của bạn", message);

            ViewBag.SuccessMessage = "Mật khẩu mới đã được gửi đến email của bạn.";
            return View();
        }

        private string GenerateRandomPassword(int length = 8)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async Task SendEmail(string toEmail, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("truongminhduc4002@gmail.com", "hocekpuhklqvkniu"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("truongminhduc4002@gmail.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                // Xử lý lỗi gửi email
                Console.WriteLine($"SMTP Exception: {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}



