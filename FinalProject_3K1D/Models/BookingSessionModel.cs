namespace FinalProject_3K1D.Models
{
    public class BookingSessionModel
    {
        public string MovieId { get; set; }
        public List<string> SelectedTimes { get; set; }
        public List<string> SelectedSeats { get; set; }
        public string UserId { get; set; } // Add this to store user ID

    }
}
