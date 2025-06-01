using eStore.Models.Category;
using eStore.Models.Feedback;
using eStore.Models.Product;

namespace eStore.Models.Home
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
