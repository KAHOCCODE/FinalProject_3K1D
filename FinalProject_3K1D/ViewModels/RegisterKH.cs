using System.ComponentModel.DataAnnotations;

namespace FinalProject_3K1D.ViewModels
{
    public class RegisterKH
    {
        [Required]
        [Display(Name = "Full Name")]
        public string HoTen { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string SDT { get; set; }

        [Required]
        [Display(Name = "ID Card Number")]
        public string CCCD { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserKH { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PassKH { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("PassKH", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassKH { get; set; }
    }
}
