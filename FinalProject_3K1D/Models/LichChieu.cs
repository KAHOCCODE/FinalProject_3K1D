﻿using System;
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
    //lấy tên phim
    public string TenPhim => IdPhimNavigation?.TenPhim ?? "Không xác định";
    //lấy tên phòng chiếu
    public string TenPhong=> IdPhongChieuNavigation.TenPhong;
    //lấy tên rạp
    public string TenRap => IdRapNavigation?.TenRap ?? "Không xác định";

}
