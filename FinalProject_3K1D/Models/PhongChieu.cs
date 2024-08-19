using System;
using System.Collections.Generic;

namespace FinalProject_3K1D.Models;

public partial class PhongChieu
{
    public string IdPhongChieu { get; set; } = null!;

    public string TenPhong { get; set; } = null!;

    public string? IdRap { get; set; }

    public virtual Rap? IdRapNavigation { get; set; }

    public virtual ICollection<LichChieu> LichChieus { get; set; } = new List<LichChieu>();
    //lấy tên rạp
    public string TenRap
    {
        get
        {
            if (IdRapNavigation != null)
            {
                return IdRapNavigation.TenRap;
            }
            return "";
        }
    }


}
