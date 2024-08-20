using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinalProject_3K1D.Models;

public partial class Phim
{
    public string IdPhim { get; set; } = null!;
    [Display(Name = "Tên Phim")]
    public string TenPhim { get; set; } = null!;
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }
    [Display(Name = "Thời lượng")]
    public double ThoiLuong { get; set; }
    [Display(Name = "Ngày khởi chiếuh")]
    public DateOnly NgayKhoiChieu { get; set; }
    [Display(Name = "Ngày kết thúc")]
    public DateOnly NgayKetThuc { get; set; }
    [Display(Name = "Quốc gia")]
    public string QuocGiaSanXuat { get; set; } = null!;
    [Display(Name = "Đạo diễn")]
    public string? DaoDien { get; set; }
    [Display(Name = "Năm sản xuât")]
    public int NamSx { get; set; }
    [Display(Name = "Định dạng phim")]
    public string? DinhDangPhim { get; set; }
    [Display(Name = "Áp phích")]
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
    //lấy giá vé từ lich chiếu  decimal
    public decimal GiaVe
    {
        get
        {
            decimal giaVe = 0;
            foreach (var lichChieu in LichChieus)
            {
                giaVe += lichChieu.GiaVe;
            }

            return giaVe;
        }
    }


    //lấy tenrap
    public string TenRap
    {
        get
        {
            string tenRap = "";
            foreach (var lichChieu in LichChieus)
            {
                tenRap += lichChieu.IdRapNavigation?.TenRap + ", ";
            }

            return tenRap;
        }
    }



}
