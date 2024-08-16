using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FinalProject_3K1D.Models
{
    public partial class NhanVien
    {
        [Required(ErrorMessage = "Mã nhân viên là bắt buộc.")]
        public string IdNhanVien { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string HoTen { get; set; } = null!;

        [DisplayName("Ngày sinh")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        public DateTime? NgaySinh { get; set; }

        [DisplayName("Địa chỉ")]
        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự.")]
        public string? DiaChi { get; set; }

        [DisplayName("Số điện thoại")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Số điện thoại chỉ được chứa các chữ số.")]
        public string? Sdt { get; set; }

        [DisplayName("Hình ảnh")]
        public string? HinhAnh { get; set; }

        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string? Email { get; set; }

        [DisplayName("Tên đăng nhập")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự.")]
        public string? UserNv { get; set; }

        [DisplayName("Mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự.")]
        public string? PassNv { get; set; }

        [DisplayName("Rạp")]
        public string? IdRap { get; set; }

        [DisplayName("Chức vụ")]
        public int? IdChucVu { get; set; }

        [DisplayName("Chức vụ")]
        public virtual ChucVu? IdChucVuNavigation { get; set; }

        [DisplayName("Rạp")]
        public virtual Rap? IdRapNavigation { get; set; }
    }
}
