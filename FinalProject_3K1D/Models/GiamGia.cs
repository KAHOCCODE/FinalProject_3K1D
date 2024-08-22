using System;
using System.Collections.Generic;

namespace FinalProject_3K1D.Models
{
    public class KhuyenMai
    {
        public int IdKhuyenMai { get; set; }
        public string TenKhuyenMai { get; set; }
        public decimal GiaTri { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }

        // Thêm mối quan hệ với KhachHang (nếu cần)
        public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
    }
}
