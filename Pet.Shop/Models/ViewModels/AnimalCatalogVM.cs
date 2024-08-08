namespace PetShop.Models.ViewModels
{
    public class AnimalCatalogVM
    {
        public IEnumerable<Animal>? Animals { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string? SelectedSortOrder { get; set; }
        public string? IsDescending { get; set; }
    }
}
