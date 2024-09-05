using System.ComponentModel.DataAnnotations;

namespace FinalProject_3K1D.Models
{
    public class Order
    {

        [Key]
        public int Idsanpham { get; set; }
        public string Tensanpham { get; set; }
        public string Mota { get; set; }
        public decimal Apphich { get; set; }
        public string PLoai { get; set; }
    }
}
