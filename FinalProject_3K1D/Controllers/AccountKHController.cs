using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProject_3K1D.Models;
using FinalProject_3K1D.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using Microsoft.AspNetCore.Identity;

namespace FinalProject_3K1D.Controllers
{
    public class AccountKHController : Controller
    {
        private readonly QlrapPhimContext _context;

        public AccountKHController(QlrapPhimContext context)
        {
            _context = context;
        }

        #region
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
                // Kiểm tra xem tên người dùng đã tồn tại chưa
                var existingUser = await _context.KhachHangs
                    .FirstOrDefaultAsync(kh => kh.UserKh == model.UserKH);

                if (existingUser != null)
                {
                    ModelState.AddModelError("UserKH", "Username already exists.");
                    return View(model);
                }

                // Kiểm tra xem SDT đã tồn tại chưa
                var existingSdt = await _context.KhachHangs
                    .FirstOrDefaultAsync(kh => kh.Sdt == model.SDT);

                if (existingSdt != null)
                {
                    ModelState.AddModelError("SDT", "Phone number already exists.");
                    return View(model);
                }

                // Kiểm tra xem CCCD đã tồn tại chưa
                var existingCccd = await _context.KhachHangs
                    .FirstOrDefaultAsync(kh => kh.Cccd == model.CCCD);

                if (existingCccd != null)
                {
                    ModelState.AddModelError("CCCD", "Identity card number already exists.");
                    return View(model);
                }

                // Kiểm tra xem Email đã tồn tại chưa
                var existingEmail = await _context.KhachHangs
                    .FirstOrDefaultAsync(kh => kh.Email == model.Email);

                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(model);
                }

                // Tạo người dùng mới và thêm vào cơ sở dữ liệu
                string newId = "KH01"; // Default ID if no users exist
                var lastUser = await _context.KhachHangs
                    .OrderByDescending(kh => kh.IdKhachHang)
                    .FirstOrDefaultAsync();

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

                var newUser = new KhachHang
                {
                    IdKhachHang = newId,
                    HoTen = model.HoTen,
                    NgaySinh = model.NgaySinh,
                    Sdt = model.SDT,
                    Cccd = model.CCCD,
                    Email = model.Email,
                    UserKh = model.UserKH,
                    PassKh = model.PassKH
                };

                try
                {
                    await _context.KhachHangs.AddAsync(newUser);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                    return RedirectToAction("Login", "AccountKH");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the user. Please try again.");
                    return View(model);
                }
            }

            return View(model);
        }



        #endregion

        #region Login in
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
                    // Login logic for customer
                    var khachhang = await _context.KhachHangs
                        .SingleOrDefaultAsync(kh => kh.UserKh == model.UserKh && kh.PassKh == model.PassKh);

                    if (khachhang == null)
                    {
                        ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                        return View(model);
                    }

                    // Store IdKhachHang in session
                    HttpContext.Session.SetString("UserId", khachhang.IdKhachHang);
                    HttpContext.Session.SetString("UserName", khachhang.HoTen);
                    HttpContext.Session.SetString("UserRole", "KhachHang");

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, khachhang.HoTen),
                new Claim(ClaimTypes.Email, khachhang.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, "KhachHang")
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(claimsPrincipal);

                    TempData["SuccessMessage"] = "Đăng nhập thành công!";
                    return RedirectToAction("Index", "Home");
                }
                else if (model.Role == "NhanVien")
                {
                    // Login logic for employee
                    var nhanvien = await _context.NhanViens
                        .SingleOrDefaultAsync(nv => nv.UserNv == model.UserKh && nv.PassNv == model.PassKh);

                    if (nhanvien == null)
                    {
                        ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                        return View(model);
                    }

                    // Store IdNhanVien in session
                    HttpContext.Session.SetString("UserId", nhanvien.IdNhanVien);
                    HttpContext.Session.SetString("UserName", nhanvien.HoTen);
                    HttpContext.Session.SetString("UserRole", "NhanVien");

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, nhanvien.HoTen),
                new Claim(ClaimTypes.Email, nhanvien.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, "NhanVien")
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

        #region đăng xuất 
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
    }
}

