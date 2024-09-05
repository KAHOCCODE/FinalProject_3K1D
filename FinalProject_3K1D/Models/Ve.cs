using System;
using System.Collections.Generic;

namespace FinalProject_3K1D.Models;

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

    public virtual KhachHang? IdKhachHangNavigation { get; set; }

    public virtual LichChieu? IdLichChieuNavigation { get; set; }
   

    //lấy tên khách hàng
    public string HoTen
    {
        get
        {
            if (IdKhachHangNavigation != null)
            {
                return IdKhachHangNavigation.HoTen;
            }
            else
            {
                return "Unknown";
            }
        }
    }
    //lấy tên phim
    public string TenPhim
    {
        get
        {
            if (IdLichChieuNavigation != null)
            {
                if (IdLichChieuNavigation.IdPhimNavigation != null)
                {
                    return IdLichChieuNavigation.IdPhimNavigation.TenPhim;
                }
                else
                {
                    return "Unknown";
                }
            }
            else
            {
                return "Unknown";
            }
        }
    }
    //tên rạp
    public string TenRap
    {
        get
        {
            if (IdLichChieuNavigation != null && IdLichChieuNavigation.IdRapNavigation != null)
            {
                return IdLichChieuNavigation.IdRapNavigation.TenRap;
            }
            else
            {
                return "Unknown";
            }
        }
    }
    public string TenPhong
    {
        get
        {
            if (IdLichChieuNavigation != null && IdLichChieuNavigation.IdPhongChieuNavigation != null)
            {
                return IdLichChieuNavigation.IdPhongChieuNavigation.TenPhong;
            }
            else
            {
                return "Unknown";
            }
        }
    }
    //lấy giờ chiếu datetime
    public DateTime GioChieu
    {
        get
        {
            if (IdLichChieuNavigation != null)
            {
                return IdLichChieuNavigation.GioChieu;
            }
            else
            {
                return DateTime.Now;
            }
        }
    }

}
