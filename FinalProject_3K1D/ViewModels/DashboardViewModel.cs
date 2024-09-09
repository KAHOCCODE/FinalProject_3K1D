using System.Collections.Generic;

namespace FinalProject_3K1D.Models
{
    public class DashboardViewModel
    {
        public int TotalCustomers { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalMovies { get; set; }
        public Dictionary<string, decimal> RevenueByTheater { get; set; }

        // Thêm thuộc tính mới
        public List<TheaterTopMovies> TopMoviesByTheater { get; set; }

        public class MovieRevenue
        {
            public string MovieTitle { get; set; }
            public decimal TotalRevenue { get; set; }
        }

        public class TheaterTopMovies
        {
            public string TheaterName { get; set; }
            public List<MovieRevenue> TopMovies { get; set; }
        }
    }
}
