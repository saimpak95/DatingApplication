using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.ViewModels
{
   public class PhotoForCreatingViewModel
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicID { get; set; }
        public PhotoForCreatingViewModel()
        {
            DateAdded = DateTime.Now;
        }
    }
}
