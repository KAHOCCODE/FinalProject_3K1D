using System;
using System.Collections.Generic;

namespace FinalProject_3K1D.Models;

public partial class Phim
{
    public string IdPhim { get; set; } = null!;

    public string TenPhim { get; set; } = null!;

    public string? MoTa { get; set; }

    public double ThoiLuong { get; set; }

    public DateOnly NgayKhoiChieu { get; set; }

    public DateOnly NgayKetThuc { get; set; }

    public string QuocGiaSanXuat { get; set; } = null!;

    public string? DaoDien { get; set; }

    public int NamSx { get; set; }

    public string? DinhDangPhim { get; set; }

    public string? ApPhich { get; set; }

    public virtual ICollection<LichChieu> LichChieus { get; set; } = new List<LichChieu>();

    public virtual ICollection<TheLoai> IdTheLoais { get; set; } = new List<TheLoai>();
    public string TenTheoLoai
    {
        get
        {
            string tenTheLoai = "";
            foreach (var theLoai in IdTheLoais)
            {
                tenTheLoai += theLoai.TenTheLoai + ", ";
            }

            return tenTheLoai;
        }
    }
    public string GioChieu
    {
        get
        {
            string gioChieu = "";
            foreach (var lichChieu in LichChieus)
            {
                gioChieu += lichChieu.GioChieu.ToString("dd/MM/yyyy") + ", ";
            }

            return gioChieu;
        }
    }
    //từ idphim của bảnh phim lấy idlichchieu từ bảng lichchieu sau đó dùng idlichchieu để lấy dữ liệu id phongchieu và sau đó lấy tenphong
    public string TenPhong
    {
        get
        {
            string tenPhong = "";
            foreach (var lichChieu in LichChieus)
            {
                tenPhong += lichChieu.IdPhongChieuNavigation.TenPhong + ", ";
            }

            return tenPhong;
        }
    }


}
