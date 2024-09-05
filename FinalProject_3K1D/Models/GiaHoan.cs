namespace FinalProject_3K1D.Models
{
    public class GiaHoan
    {
        public int Id { get; set; } // Auto-incrementing primary key

        public int ThoiGianHoan { get; set; } // Time in hours

        public double PhanTramHoan { get; set; } // Refund percentage (e.g., 80 for 80%)
    }
}
