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
    public string PhimName
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
    
}
