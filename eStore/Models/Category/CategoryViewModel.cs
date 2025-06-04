using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace eStore.Models.Category
{
    public class CategoryViewModel
    {
        [Ignore]
        public Guid CategoryID { get; set; }
        [Display(Name = "Danh mục")]
        public string CategoryName { get; set; }

        [Ignore]
        public bool CanDelete { get; set; } = true;
    }
}
