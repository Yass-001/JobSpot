using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobSpot.Models
{
    public class JobPosting
    {
        [Key] // Primary key attribute - maybe not needed if convention is followed
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public string Location { get; set; }

        public DateTime PostedDate { get; set; } = DateTime.UtcNow; // - ?! Default to current date when creating a new posting

        public bool IsApproved { get; set; } = false;
        [Required]
        public string UserId { get; set; } // ? Foreign key to the User who posted the job
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; } // Navigation property to the User entity




    }
}
