namespace ITSupportLogbook.ViewModels
{
    public class UpdateStatusViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; } = "Pending";
        public string? ResolutionNotes { get; set; }
    }
}