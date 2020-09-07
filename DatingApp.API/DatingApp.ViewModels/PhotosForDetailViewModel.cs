using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.ViewModels
{
   public class PhotosForDetailViewModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string DateAdded { get; set; }
        public bool IsMain { get; set; }
    }
}
