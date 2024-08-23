namespace FinalProject_3K1D.Models
{
    public class PaymentDetails
    {
        public List<string> SeatIds { get; set; }
        public int TotalAmount { get; set; }
        public string TenRap { get; set; }     // Added Cinema Name
        public string TenPhong { get; set; }   // Added Room Name
    }
}
