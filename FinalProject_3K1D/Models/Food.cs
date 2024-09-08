using System.ComponentModel.DataAnnotations;

namespace FinalProject_3K1D.Models
{
    public class Food
    {
        [Key]
        public int IdSanPham { get; set; }  // Đã xác định là khóa chính

        public string TenSanPham { get; set; }
        public decimal Gia { get; set; }
        public string MoTa { get; set; }
        public string Apphich { get; set; }
        public string PLoai { get; set; }
    }
}
