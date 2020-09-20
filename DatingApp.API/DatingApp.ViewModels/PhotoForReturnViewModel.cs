namespace DatingApp.ViewModels
{
    public class PhotoForReturnViewModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicID { get; set; }
    }
}