using System.ComponentModel.DataAnnotations;

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
        
        public DateTime PostedDate { get; set; } // - ?! Default to current date when creating a new posting


        public JobPosting() { }



    }
}
