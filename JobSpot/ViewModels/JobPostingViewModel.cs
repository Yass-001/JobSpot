using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobSpot.ViewModels
{
    /// <summary>
    /// ViewModel for creating and editing job postings
    /// </summary>
    public class JobPostingViewModel
    {
        [ReadOnly(true)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        [Display(Name = "Job Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(5000, MinimumLength = 10)]
        [Display(Name = "Job Description")]
        public string Description { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 2)]
        [Display(Name = "Company Name")]
        public string Company { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 2)]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [StringLength(100)]
        [Display(Name = "Job Category")]
        public string Category { get; set; }

        [Display(Name = "Minimum Salary")]
        [Range(0, 9999999)]
        [DataType(DataType.Currency)]
        public decimal? SalaryMin { get; set; }

        [Display(Name = "Maximum Salary")]
        [Range(0, 9999999)]
        [DataType(DataType.Currency)]
        public decimal? SalaryMax { get; set; }

        [StringLength(50)]
        [Display(Name = "Salary Currency")]
        public string SalaryCurrency { get; set; } = "EUR";
    }
}
