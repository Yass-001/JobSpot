using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace JobSpot.Models
{
    public class JobPosting
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(5000)]
        public string Description { get; set; }

        [Required]
        [StringLength(150)]
        public string Company { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; }

        /// <summary>
        /// Job category/type (e.g., "IT", "Finance", "Marketing", "Sales", etc.)
        /// </summary>
        [StringLength(100)]
        public string Category { get; set; }

        /// <summary>
        /// Minimum salary for this position
        /// </summary>
        [Range(0, 9999999)]
        public decimal? SalaryMin { get; set; }

        /// <summary>
        /// Maximum salary for this position
        /// </summary>
        [Range(0, 9999999)]
        public decimal? SalaryMax { get; set; }

        /// <summary>
        /// Currency code (e.g., "USD", "EUR", "GBP")
        /// </summary>
        [StringLength(50)]
        public string SalaryCurrency { get; set; } = "EUR";

        /// <summary>
        /// Date and time when this job was posted
        /// </summary>
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Whether this job posting has been approved for display
        /// </summary>
        //[Index("IX_IsApproved", IsUnique = false)] - ? EF Core does not support Index attribute, we will create index via Fluent API in DbContext (?)
        public bool IsApproved { get; set; } = false;

        [Required]
        public string UserId { get; set; } // Foreign key to the User who posted the job

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; } // Navigation property to the User entity

        //public string ToString(decimal salary, CultureInfo culture)
        //{
        //    return salary.ToString("C:0", culture);
        //}
    }
}
