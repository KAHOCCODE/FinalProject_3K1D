using System;
using System.Collections.Generic;

namespace FinalProject_3K1D.Models;

public partial class ChucVu
{
    public int IdChucVu { get; set; }

    public string? TenChucVu { get; set; }

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
