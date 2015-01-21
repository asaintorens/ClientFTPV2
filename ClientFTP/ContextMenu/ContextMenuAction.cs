using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientFTP
{
   public enum ContextMenuActionClient
    {
        SelectAll ,
       CopySelection,
       Uploader
    }
   public enum ContextMenuActionDistant
   {
       SelectAll,
       Telecharger,
       CreerDossier,
       Renommer,
       Supprimer
   }
}
