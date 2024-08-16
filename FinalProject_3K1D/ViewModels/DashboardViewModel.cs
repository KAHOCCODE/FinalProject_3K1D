using System.Collections.Generic;

namespace FinalProject_3K1D.Models
{
    public class DashboardViewModel
    {
        public int TotalCustomers { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalMovies { get; set; }
        public Dictionary<string, decimal> RevenueByTheater { get; set; }
    }

}
