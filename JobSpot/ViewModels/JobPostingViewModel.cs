using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobSpot.ViewModels
{
    public class JobPostingViewModel
    {
        [ReadOnly(true)]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public string Location { get; set; }
    }
}
