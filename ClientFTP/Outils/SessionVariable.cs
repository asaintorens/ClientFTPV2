using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientFTP
{
   public class SessionVariable
    {
        public string LastFolderSelectedDistant 
        { get; set ; }
        public string rootPathClient { get; set; } 
        public string LastFolderSelectedClient { get; set; }

       public SessionVariable()
        {
            this.rootPathClient = null;
        }
    }
}
