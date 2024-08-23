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

    public string? IdPhim { get; set; }

    public string? IdRap { get; set; }

    public virtual Phim? IdPhimNavigation { get; set; }

    public virtual Rap? IdRapNavigation { get; set; } 

    public virtual PhongChieu IdPhongChieuNavigation { get; set; } = null!;

    public virtual ICollection<Ve> Ves { get; set; } = new List<Ve>();
    //lấy tên phòng
    public string TenPhong
    {
        get
        {
            if (IdPhongChieuNavigation != null)
            {
                return IdPhongChieuNavigation.TenPhong;
            }
            else
            {
                return "Unknown";
            }
        }
    }
    //lấy tên rạp

    public string TenRap
    {
        get
        {
            if (IdRapNavigation != null)
            {
                return IdRapNavigation.TenRap;
            }
            else
            {
                return "Unknown";
            }
        }
    }
    



}
