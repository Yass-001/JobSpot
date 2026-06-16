using System.ComponentModel.DataAnnotations;

namespace JobSpot.ViewModels
{
    /// <summary>
    /// ViewModel for job search and filtering UI form
    /// Handles all search, filter, sort, and pagination parameters
    /// </summary>
    public class JobSearchFilterViewModel
    {
        // ===== Search Parameters =====

        /// <summary>
        /// Free-text search across Title, Company, Location, and Description
        /// </summary>
        [Display(Name = "Search (Title, Company, Location)")]
        [StringLength(200)]
        public string? SearchQuery { get; set; }

        // ===== Filter Parameters =====

        /// <summary>
        /// Filter by job category
        /// </summary>
        [Display(Name = "Category")]
        [StringLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Minimum salary filter
        /// </summary>
        [Display(Name = "Minimum Salary")]
        [Range(0, 9999999)]
        public decimal? SalaryMin { get; set; }

        /// <summary>
        /// Maximum salary filter
        /// </summary>
        [Display(Name = "Maximum Salary")]
        [Range(0, 9999999)]
        public decimal? SalaryMax { get; set; }

        /// <summary>
        /// Filter jobs posted within last N days
        /// </summary>
        [Display(Name = "Posted within last (days)")]
        [Range(1, 365)]
        public int? PostedWithinDays { get; set; }

        // ===== Sorting =====

        /// <summary>
        /// How to sort the results
        /// </summary>
        [Display(Name = "Sort by")]
        public JobSortOption SortBy { get; set; } = JobSortOption.Newest;

        // ===== Pagination =====

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        [Range(1, 1000)]
        [Display(Name = "Page")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of items per page
        /// </summary>
        [Range(5, 100)]
        [Display(Name = "Items per page")]
        public int PageSize { get; set; } = 10;

        // ===== UI Data =====

        /// <summary>
        /// Available categories for dropdown (populated by controller)
        /// </summary>
        [Display(Name = "Available Categories")]
        public List<string> AvailableCategories { get; set; } = new();
    }

    /// <summary>
    /// Enum for sorting options
    /// Take it to folder "Enums" if it grows too big, but for now it's small and closely related to the search/filtering, so it can stay here ?!
    /// </summary>
    public enum JobSortOption
    {
        [Display(Name = "Newest First")]
        Newest = 0,

        [Display(Name = "Oldest First")]
        Oldest = 1,

        [Display(Name = "Salary: High to Low")]
        SalaryHighToLow = 2,

        [Display(Name = "Salary: Low to High")]
        SalaryLowToHigh = 3,

        [Display(Name = "Company: A-Z")]
        CompanyAZ = 4,

        [Display(Name = "Company: Z-A")]
        CompanyZA = 5,

        [Display(Name = "Title: A-Z")]
        TitleAZ = 6,

        [Display(Name = "Title: Z-A")]
        TitleZA = 7
    }
}
