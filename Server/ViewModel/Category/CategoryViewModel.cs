using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Server.ViewModel.Category
{
    public class CategoryViewModel
    {
        [Ignore]
        public Guid CategoryID { get; set; }
        [Display(Name = "Danh mục")]
        public string CategoryName { get; set; }

        public bool CanDelete { get; set; } = true;
    }
}
