using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinalProject_3K1D.Models
{
    public class Food
    {
        [Key]
        public int IdSanPham { get; set; }

        public string TenSanPham { get; set; }

        public string MoTa { get; set; }

        public string Apphich { get; set; } // Path to image

        public decimal Gia { get; set; }

        public string PLoai { get; set; } // Category: 'nước', 'bắp', 'combo'

        public virtual ICollection<Ve> Ves { get; set; }
    }
}
