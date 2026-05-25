using System;
using System.ComponentModel.DataAnnotations;

namespace ITSupportLogbook.Models.Entities
{
    public class Issue
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Officer Name")]
        public string OfficerName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Extension Number")]
        public string Extension { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Nature of Issue")]
        public string IssueDescription { get; set; } = string.Empty;

        //public string Status { get; set; } = "Pending";
        [Display(Name = "Urgency")]
        public string Status { get; set; } = "Low";

        [Display(Name = "Date Reported")]
        public DateTime DateReported { get; set; } = DateTime.UtcNow;

        [Display(Name = "Date Resolved")]
        public DateTime? DateResolved { get; set; } 

        [Display(Name = "Resolution Notes")]
        public string? ResolutionNotes { get; set; }


    }
}
