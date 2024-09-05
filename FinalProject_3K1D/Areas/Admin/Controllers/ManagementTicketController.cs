using Microsoft.AspNetCore.Mvc;
using FinalProject_3K1D.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_3K1D.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManagementTicketController : Controller
    {
        private readonly QlrapPhimContext _context;

        public ManagementTicketController(QlrapPhimContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            // Fetch all ticket data with related entities
            var tickets = _context.Ves
                 .Where(v => v.TrangThai == 1)
                .Select(v => new
                {
                    v.IdVe,
                    TenPhim = v.IdLichChieuNavigation.IdPhimNavigation.TenPhim,
                    HoTen = v.IdKhachHangNavigation.HoTen,
                    v.MaGheNgoi,
                    v.TienBanVe,
                    v.NgayMua,
                    GioChieu = v.IdLichChieuNavigation.GioChieu,
                    TenPhong = v.IdLichChieuNavigation.IdPhongChieuNavigation.TenPhong
                });

            // Filter tickets by movie name if a search string is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                tickets = tickets.Where(t => t.TenPhim.Contains(searchString));
            }

            // Pass the data to the view
            return View(tickets.ToList());
        }
        public IActionResult HuyVe(string searchString1)
        {
            // Fetch all ticket data with related entities
            var tickets = _context.Ves
                 .Where(v => v.TrangThai == 0)
                .Select(v => new
                {
                    v.IdVe,
                    TenPhim = v.IdLichChieuNavigation.IdPhimNavigation.TenPhim,
                    HoTen = v.IdKhachHangNavigation.HoTen,
                    v.MaGheNgoi,
                    v.TienBanVe,
                    v.NgayMua,
                    GioChieu = v.IdLichChieuNavigation.GioChieu,
                    TenPhong = v.IdLichChieuNavigation.IdPhongChieuNavigation.TenPhong
                });

            // Filter tickets by movie name if a search string is provided
            if (!string.IsNullOrEmpty(searchString1))
            {
                tickets = tickets.Where(t => t.TenPhim.Contains(searchString1));
            }

            // Pass the data to the view
            return View(tickets.ToList());
        }
        public IActionResult VeDaXem(string searchString2)
        {
            // Fetch all ticket data with related entities
            var tickets = _context.Ves
                 .Where(v => v.TrangThai == 2)
                .Select(v => new
                {
                    v.IdVe,
                    TenPhim = v.IdLichChieuNavigation.IdPhimNavigation.TenPhim,
                    HoTen = v.IdKhachHangNavigation.HoTen,
                    v.MaGheNgoi,
                    v.TienBanVe,
                    v.NgayMua,
                    GioChieu = v.IdLichChieuNavigation.GioChieu,
                    TenPhong = v.IdLichChieuNavigation.IdPhongChieuNavigation.TenPhong
                });

            // Filter tickets by movie name if a search string is provided
            if (!string.IsNullOrEmpty(searchString2))
            {
                tickets = tickets.Where(t => t.TenPhim.Contains(searchString2));
            }

            // Pass the data to the view
            return View(tickets.ToList());
        }
        public IActionResult HoanVe(string searchString3)
        {
 
                 var tickets = _context.Ves
                 .Where(v => v.TrangThai == 3 || v.TrangThai == 4 || v.TrangThai == 5 || v.TrangThai==6)
                .Select(v => new
                {
                    v.IdVe,
                    TenPhim = v.IdLichChieuNavigation.IdPhimNavigation.TenPhim,
                    HoTen = v.IdKhachHangNavigation.HoTen,
                    v.MaGheNgoi,
                    v.TienBanVe,
                    v.NgayMua,
                    GioChieu = v.IdLichChieuNavigation.GioChieu,
                    TenPhong = v.IdLichChieuNavigation.IdPhongChieuNavigation.TenPhong,
                    NoiDung = v.NoiDung
                });

            // Filter tickets by movie name if a search string is provided
            if (!string.IsNullOrEmpty(searchString3))
            {
                tickets = tickets.Where(t => t.TenPhim.Contains(searchString3));
            }

            // Pass the data to the view
            return View(tickets.ToList());
        }

    }
    //[HttpPost]
    //public IActionResult UpdateTicketStatus([FromBody] TicketStatusUpdateModel model)
    //{
    //    var result = new { Success = false, Message = string.Empty };

    //    try
    //    {
    //        var ticket = _context.Ves.Find(model.IdVe);
    //        if (ticket == null)
    //        {
    //            result = new { Success = false, Message = "Ticket not found." };
    //            return Json(result);
    //        }

    //        // Check if this is a refund confirmation (status 0)
    //        if (model.Status == 0)
    //        {
    //            // Calculate loyalty points based on ticket price
    //            decimal loyaltyPoints = ticket.TienBanVe * 0.001m;

    //            // Retrieve the customer
    //            var customer = _context.KhachHangs.Find(ticket.IdKhachHang);
    //            if (customer != null)
    //            {
    //                // Add the calculated points to the customer's existing loyalty points
    //                customer.DiemTichLuy += loyaltyPoints;

    //                // Save changes to customer loyalty points
    //                _context.SaveChanges();
    //            }
    //        }
    //        else if (model.Status == 7)
    //        {
    //            // In case of cancellation (status 7), subtract the loyalty points
    //            decimal loyaltyPointsToDeduct = ticket.TienBanVe * 0.001m;

    //            var customer = _context.KhachHangs.Find(ticket.IdKhachHang);
    //            if (customer != null)
    //            {
    //                // Deduct the points if applicable
    //                customer.DiemTichLuy = Math.Max(0, customer.DiemTichLuy - loyaltyPointsToDeduct);

    //                // Save changes to customer loyalty points
    //                _context.SaveChanges();
    //            }
    //        }

    //        // Update the ticket status
    //        ticket.TrangThai = model.Status;
    //        _context.SaveChanges();

    //        result = new { Success = true, Message = "Trạng thái vé và điểm tích lũy đã được cập nhật thành công." };
    //    }
    //    catch (Exception ex)
    //    {
    //        result = new { Success = false, Message = $"Error updating ticket status: {ex.Message}" };
    //    }

    //    return Json(result);
    //}

}
