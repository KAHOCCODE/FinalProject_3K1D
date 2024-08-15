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
                    if (khachhang.PassKh == model.PassKh)
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
                            return RedirectToAction("_Home");
                        }
                    }
                }
            }
            return View();
        }
        #endregion
    }
}
