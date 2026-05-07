using System;
using System.ComponentModel.DataAnnotations;

namespace ITSupportLogbook.Models
{
    public class Issue
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Officer Name")]
        public string OfficerName { get; set; }

        [Required]
        [Display(Name = "Extension Number")]
        public string Extension { get; set; }

        [Required]
        [Display(Name = "Nature of Issue")]
        public string IssueDescription { get; set; }

        public string Status { get; set; } = "Pending";

        [Display(Name = "Date Reported")]
        public DateTime DateReported { get; set; } = DateTime.Now;

        [Display(Name = "Date Resolved")]
        public DateTime? DateResolved { get; set; }
    }
}
