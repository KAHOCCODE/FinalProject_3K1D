using System.ComponentModel.DataAnnotations;

namespace FinalProject_3K1D.ViewModels
{
    public class LoginKH
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Username")]
        public string UserKh { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string PassKh { get; set; }

        public bool RememberMe { get; set; }
    
    }
}
