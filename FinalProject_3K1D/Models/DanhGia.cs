using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinalProject_3K1D.Models
{
    public class DanhGia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDanhGia { get; set; }

        [ForeignKey("Phim")]
        public string IdPhim { get; set; }
        public Phim Phim { get; set; }

        [ForeignKey("KhachHang")]
        public string IdKhachHang { get; set; }
        public KhachHang KhachHang { get; set; }

        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        public string NoiDung { get; set; }

        [Range(1, 5)]
        public int Diem { get; set; }

        public bool TrangThaiDanhGia { get; set; } = false; // 0: Pending, 1: Approved
    }
}
