using System;
using System.Collections.Generic;

namespace FinalProject_3K1D.Models
{
    public partial class Ve
    {
        public int IdVe { get; set; }

        public int? LoaiVe { get; set; }

        public string? MaGheNgoi { get; set; }

        public string? IdKhachHang { get; set; }

        public int TrangThai { get; set; }

        public decimal? TienBanVe { get; set; }

        public DateTime? NgayMua { get; set; }

        public string? IdLichChieu { get; set; }

        public string? NoiDung { get; set; }

        public int? IdSanPham { get; set; } // Foreign Key to Food

        public virtual KhachHang? IdKhachHangNavigation { get; set; }

        public virtual LichChieu? IdLichChieuNavigation { get; set; }

        public virtual Food? SanPhamNavigation { get; set; } // Navigation property for Food

        // Get customer's full name
        public string HoTen
        {
            get
            {
                return IdKhachHangNavigation?.HoTen ?? "Unknown";
            }
        }

        // Get movie name
        public string TenPhim
        {
            get
            {
                return IdLichChieuNavigation?.IdPhimNavigation?.TenPhim ?? "Unknown";
            }
        }

        // Get theater name
        public string TenRap
        {
            get
            {
                return IdLichChieuNavigation?.IdRapNavigation?.TenRap ?? "Unknown";
            }
        }

        // Get room name
        public string TenPhong
        {
            get
            {
                return IdLichChieuNavigation?.IdPhongChieuNavigation?.TenPhong ?? "Unknown";
            }
        }

        // Get showtime
        public DateTime GioChieu
        {
            get
            {
                return IdLichChieuNavigation?.GioChieu ?? DateTime.Now;
            }
        }
    }
}
