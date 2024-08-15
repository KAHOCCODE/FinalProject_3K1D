using System;
using System.Collections.Generic;

namespace FinalProject_3K1D.Models;

public partial class Rap
{
    public string IdRap { get; set; } = null!;

    public string? TenRap { get; set; }

    public string? DiaChi { get; set; }

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();

    public virtual ICollection<PhongChieu> PhongChieus { get; set; } = new List<PhongChieu>();
}
