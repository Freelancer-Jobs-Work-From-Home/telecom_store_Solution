

using Server.ViewModel.Category;
using Server.ViewModel.Feedback;
using Server.ViewModel.Product;

namespace Server.ViewModel.Home
{
    public class HomeViewModel
    {
        public List<ProductViewModel> TelecomProducts { get; set; } 
        public List<ProductViewModel> ConsumerProducts { get; set; }
        public List<CategoryViewModel> Categories { get; set; }

        public List<FeedbackViewModel>? Feedbacks { get; set; }

        public FeedbackViewModel? FeedbackViewModel { get; set; }
    }
}
