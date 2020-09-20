using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.ViewModels
{
   public class MessageForCreationViewModel
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public DateTime MessageSent { get; set; }
        public string Content { get; set; }

        public MessageForCreationViewModel()
        {
            MessageSent = DateTime.Now;
        }

    }
}
