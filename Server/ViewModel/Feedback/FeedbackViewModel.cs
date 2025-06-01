namespace Server.ViewModel.Feedback
{
    public class FeedbackViewModel
    {
        public Guid FeedbackID { get; set; }

        public string FullName { get; set; }
        public Guid UserID { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Email { get; set; }
    }
}
