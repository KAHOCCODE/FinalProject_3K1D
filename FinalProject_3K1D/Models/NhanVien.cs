using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FinalProject_3K1D.Models;

public partial class NhanVien
{
    public string IdNhanVien { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateTime? NgaySinh { get; set; }

    public string? DiaChi { get; set; }

    public string? Sdt { get; set; }

    public string? HinhAnh { get; set; }

    public string? Email { get; set; }

    public string? UserNv { get; set; }

    public string? PassNv { get; set; }

    public string? IdRap { get; set; }

    public int? IdChucVu { get; set; }
    [DisplayName("Chức vụ")]

    public virtual ChucVu? IdChucVuNavigation { get; set; }
    [DisplayName("Rạp")]

    public virtual Rap? IdRapNavigation { get; set; }
}
