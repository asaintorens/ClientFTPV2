using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace ClientFTP.Upload
{
   public class UploadFTP
    {
       private FormFTP monForm;

       public UploadFTP(FormFTP monForm)
       {
           // TODO: Complete member initialization
           this.monForm = monForm;
       }

       public void UpLoadFile()
       {

           if (EstDossierDistantNonSelectionné())
               AfficherErreur();
           else
           {
               UploaderFichier();
           }
       }

       private void UploaderFichier()
       {
           List<string> FolderAdded = new List<string>();
           List<string> Files = new List<string>();
           string PathClientStart = monForm.sessionVariable.LastFolderSelectedClient;
           string pathServerStart = monForm.sessionVariable.LastFolderSelectedDistant;
               foreach (ListViewItem unItemSelected in monForm.listView1.SelectedItems)
               {

                   Files.Add(unItemSelected.Text);    
               }

               UploaderListe(Files);

               monForm.sessionVariable.LastFolderSelectedClient = PathClientStart;
               monForm.sessionVariable.LastFolderSelectedDistant = pathServerStart;           
       }

       private void UploaderListe(List<string> Files)
       {
           List<string> Dossiers = new List<string>();
           foreach (string file in Files)
           {
               if (EstUnDossier(file))
               {
                   CreerDossier(file);
                   Dossiers.Add(file);
               }
               else
               {
                   CreerFichier(file);
               }
           }

           foreach(string oneFolder in Dossiers)
           {
               List<string> filesName = new List<string>();
               string origineClient = monForm.sessionVariable.LastFolderSelectedClient;
               string origineServeur = monForm.sessionVariable.LastFolderSelectedDistant;

               monForm.sessionVariable.LastFolderSelectedClient+="/"+oneFolder;
               monForm.sessionVariable.LastFolderSelectedDistant+="/"+oneFolder;
               List<String> files = Directory.GetDirectories(monForm.sessionVariable.LastFolderSelectedClient.Replace('\\','/')).ToList();
               files.AddRange(( Directory.GetFiles(monForm.sessionVariable.LastFolderSelectedClient.Replace('\\','/')).ToList()));
               foreach(string oneFilePath in files)
               {
                   string fileFormated = oneFilePath.Replace('\\', '/');
                   string[] fileCasted = fileFormated.Split('/');
                   filesName.Add(fileCasted.ElementAt(fileCasted.Count()-1));
               }

               this.UploaderListe(filesName);
               monForm.sessionVariable.LastFolderSelectedClient = origineClient;
               monForm.sessionVariable.LastFolderSelectedDistant = origineServeur;

           }


       }

       private void CreerFichier(string folderName)
       {
           try
           {
               string res = this.monForm.managerFTP.Uploader(DossierSelectionnerClient(), folderName, DossierSelectionnerServeur());
               monForm.consoleManager.AppendText(res, System.Drawing.Color.Green);
           }
           catch (Exception EX)
           {

               this.monForm.consoleManager.AppendText(EX.Message, System.Drawing.Color.Red);
           }
       }

  
       public void CreerDossier(string FolderName)
       {
           try
           {
               this.monForm.consoleManager.AppendText(monForm.managerFTP.CreerDossier(DossierSelectionnerClient(), FolderName, DossierSelectionnerServeur()), System.Drawing.Color.Green
             );
           }
           catch (Exception ex)
           {
               this.monForm.consoleManager.AppendText(ex.Message, System.Drawing.Color.Red);

           }
       }


       private bool EstUnDossier(string FileName)
       {
           bool res = false;
           res = FileName.Contains('.');
           return !res;
       }

       private string DossierSelectionnerServeur()
       {
           return monForm.sessionVariable.LastFolderSelectedDistant;
       }

       private string DossierSelectionnerClient()
       {
           return monForm.sessionVariable.LastFolderSelectedClient;
       }

       private void AfficherErreur()
       {
           monForm.consoleManager.AppendText("Aucun dossier distant sélectionné.", System.Drawing.Color.Red);
       }

       private bool EstDossierDistantNonSelectionné()
       {
           return monForm.sessionVariable.LastFolderSelectedDistant == null;
       }
    }
}
