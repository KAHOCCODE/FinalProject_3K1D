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
        public IActionResult Register()
        {
            return View();
        }

        // POST: /AccountKH/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterKH model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username already exists
                var existingUser = await _context.KhachHangs
                    .FirstOrDefaultAsync(kh => kh.UserKh == model.UserKH);

                if (existingUser != null)
                {
                    ModelState.AddModelError("UserKH", "Username already exists.");
                    return View(model);
                }

                // Create new user and add to database
                var newUser = new KhachHang
                {
                    HoTen = model.HoTen,
                    NgaySinh = model.NgaySinh,
                    Sdt = model.SDT,
                    Cccd = model.CCCD,
                    Email = model.Email,
                    UserKh = model.UserKH,
                    PassKh = model.PassKH // Ensure password is hashed in a real-world application
                };

                _context.KhachHangs.Add(newUser);
                await _context.SaveChangesAsync();

                // Redirect to login page or another page as needed
                return RedirectToAction("Login", "AccountKH");
            }

            // If the model state is invalid, return to the register view with the model
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
        public async Task<IActionResult> Login(LoginKH model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachhang = _context.KhachHangs.SingleOrDefault(kh => kh.IdKhachHang == model.UserKh);
                if (khachhang == null)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại");
                    return View();
                }
                else
                {
                    if (khachhang.PassKh != model.PassKh)
                    {
                        ModelState.AddModelError("", "Mật khẩu không đúng");
                        return View();
                    }
                    else
                    {
                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, khachhang.HoTen),
                                new Claim(ClaimTypes.Email, khachhang.Email ?? string.Empty),   
                                
                                //claim -role động
                                new Claim(ClaimTypes.Role, "KhachHang")
                            };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(claimsPrincipal);
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("index");
                        }
                    }
                }
            }
            return View();
        }
        #endregion
    }
}
