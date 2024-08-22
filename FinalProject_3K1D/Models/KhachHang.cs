using System;
using System.Collections.Generic;

namespace FinalProject_3K1D.Models
{
    public partial class KhachHang
    {
        public string IdKhachHang { get; set; } = null!;

        public string HoTen { get; set; } = null!;

        public DateTime? NgaySinh { get; set; }

        public string? DiaChi { get; set; }

        public string? Sdt { get; set; }

        public string? Cccd { get; set; }

        public int? DienTichLuy { get; set; } = 0;

        public string? Email { get; set; }

        public string? UserKh { get; set; }

        public string? PassKh { get; set; }

        // Thêm thuộc tính idKhuyenMai
        public int? IdKhuyenMai { get; set; }

        // Thêm mối quan hệ với KhuyenMai
        public virtual KhuyenMai? KhuyenMai { get; set; }

        public virtual ICollection<Ve> Ves { get; set; } = new List<Ve>();
    }
}
