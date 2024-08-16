using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

namespace FinalProject_3K1D.Models;

public partial class LichChieu
{
    public string IdLichChieu { get; set; } = null!;

    public DateTime GioChieu { get; set; }

    public string IdPhongChieu { get; set; } = null!;

    public decimal GiaVe { get; set; }

    public int TrangThai { get; set; }

    public string? IdPhim { get; set; }

    public virtual Phim? IdPhimNavigation { get; set; }

    public virtual PhongChieu IdPhongChieuNavigation { get; set; } = null!;

    public virtual ICollection<Ve> Ves { get; set; } = new List<Ve>();
    public string TenPhim
    {
        get
        {
            if (IdPhimNavigation != null)
            {
                return IdPhimNavigation.TenPhim;
            }
            return string.Empty;
        }
    }
    public string TenPhong
    {
        get
        {
            if (IdPhongChieuNavigation != null)
            {
                return IdPhongChieuNavigation.TenPhong;
            }
            return string.Empty;
        }
    }
    public double ThoiLuong
    {
        get
        {
            if (IdPhimNavigation != null)
            {
                return IdPhimNavigation.ThoiLuong;
            }
            return 0;
        }
    }
    public DateOnly NgayKhoiChieu
    {
        get
        {
            if (IdPhimNavigation != null)
            {
                return IdPhimNavigation.NgayKhoiChieu;
            }
            return default(DateOnly);
        }
    }
    public DateOnly NgayKetThuc
    {
        get
        {
            if (IdPhimNavigation != null)
            {
                return IdPhimNavigation.NgayKetThuc;
            }
            else
            {
                return default(DateOnly);
            }
        }
    }
    public string DinhDangPhim
    {
        get
        {
            if (IdPhimNavigation != null)
            {
                return IdPhimNavigation.DinhDangPhim;
            }
            return string.Empty;
        }
    }

}
