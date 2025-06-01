namespace eStore.Models.Admin
{
    public class DashboardViewModel
    {
        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int UserCount { get; set; }
        public decimal MonthlyEarnings { get; set; }
        public decimal YearlyEarnings { get; set; }

        public int TotalReviews { get; set; }

        public int TotalFeedbacks { get; set; }



        public int TotalOrders => PendingOrders + CancelledOrders + CompletedOrders;
        public List<decimal> MonthlyEarningsData { get; set; } = new List<decimal>(); 
    }
}
