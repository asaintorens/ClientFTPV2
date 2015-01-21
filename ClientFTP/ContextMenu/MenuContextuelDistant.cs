using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace ClientFTP
{
    public class MenuContextuelDistant
    {
        ContextMenuStrip ContextMenu = new ContextMenuStrip();

        FormFTP monForm;
        MouseEventArgs monEvent;
        object monSender;

        public MenuContextuelDistant(FormFTP leForm, MouseEventArgs leEvent, object lesender)
        {
            monForm = leForm;
            monEvent = leEvent;
            monSender = lesender;
        }

        public void Show()
        {
            ListView item = (ListView)monSender;

            MouseEventArgs mouse = (MouseEventArgs)monEvent;
            if (mouse.Button == MouseButtons.Right)
            {
                ChargerAction();
                SetEvent();
                ContextMenu.Show(monForm.listViewDistant, mouse.Location);
            }


        }

        private void SetEvent()
        {
            ContextMenu.ItemClicked += new ToolStripItemClickedEventHandler(this.contextMenu_ItemClicked);
        }

        private void ChargerAction()
        {
            foreach (string uneAction in ContextMenuActionDistant.GetNames(typeof(ContextMenuActionDistant)))
            {
                ContextMenu.Items.Add(uneAction);
            }
        }
        private void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            if (item.Text == ContextMenuActionDistant.CreerDossier.ToString())
            {
                CreerDossier();
            }
            if (item.Text == ContextMenuActionDistant.Telecharger.ToString())
            {
                if (this.monForm.listViewDistant.SelectedItems.Count ==0)
                {
                    MessageBox.Show("Aucun fichier selectionné");
                }
                else
                {
                    TelechargerFichier(this.monForm.listViewDistant.SelectedItems);
                }
            }
            if (item.Text == ContextMenuActionDistant.SelectAll.ToString())
            {
                foreach (ListViewItem oneListViewItem in monForm.listViewDistant.Items)
                {
                    oneListViewItem.Selected = true;
                }
            }
            if(item.Text == ContextMenuActionDistant.Renommer.ToString())
            {

                if (this.monForm.listViewDistant.SelectedItems.Count != 1)
                {
                    MessageBox.Show("Veuillez sélectionner un item");
                }
                else
                {
                    this.RenomerDossier(this.monForm.listViewDistant.SelectedItems[0]);
                }
                
            }
            if(item.Text == ContextMenuActionDistant.Supprimer.ToString())
            {
                if (this.monForm.listViewDistant.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner un item");
                }
                else
                {
                    this.Supprimer(this.monForm.listViewDistant.SelectedItems);
                }

            }


            // your code here
        }

        private void Supprimer(ListView.SelectedListViewItemCollection selectedListViewItemCollection)
        {
            try
            {
                foreach (ListViewItem oneItem in selectedListViewItemCollection)
                {
                    if (oneItem.Text.Contains('.'))
                    {
                        string messageDeleted = this.monForm.managerFTP.DeleteFile(oneItem.Text, monForm.sessionVariable.LastFolderSelectedDistant);
                        this.monForm.consoleManager.AppendText(messageDeleted, this.monForm.consoleManager.green);
                    }else
                    {
                        string messageDeleted = this.monForm.managerFTP.DeleteFolder(oneItem.Text, monForm.sessionVariable.LastFolderSelectedDistant);
                        this.monForm.consoleManager.AppendText(messageDeleted, this.monForm.consoleManager.green);
                    }
                }
            }
            catch (Exception e)
            {
                this.monForm.consoleManager.AppendText(e.Message, this.monForm.consoleManager.red);
            }
        }

        private void TelechargerFichier(ListView.SelectedListViewItemCollection selectedListViewItemCollection)
        {
            try
            {
                foreach (ListViewItem oneItem in selectedListViewItemCollection)
                {
                    UploadDownload.DownloadFTP downloadFtp = new UploadDownload.DownloadFTP(this.monForm);
                    downloadFtp.Download(oneItem.Text);
                }
            }
            catch (Exception e)
            {
                this.monForm.consoleManager.AppendText(e.Message, this.monForm.consoleManager.red);
            }
           
        }

        private void CreerDossier()
        {
            string FolderName = "MonNouveauDossier";
            bool FileEstValide = false;
            while (!FileEstValide)
            {
                if ((InputBoxManager.InputBox("New document", "New document name :", ref FolderName) == DialogResult.OK))
                {
                    FileEstValide = IsValidFilename(FolderName) && FolderName.Length > 0;
                }
            }

            Upload.UploadFTP Upload = new Upload.UploadFTP(this.monForm);
            Upload.CreerDossier(FolderName);
        }
        bool IsValidFilename(string testName)
        {
            bool valide = true;
        
            if (testName.Contains('.'))
            {
                valide = false;
            }
            if (testName.Contains('*'))
            {
                valide = false;
            }

 
        return valide;
        }
        private void RenomerDossier(ListViewItem item)
        {
            string FolderName = "MonNouveauDossier";
            
            bool FileEstValide = false;
            while (!FileEstValide)
            {
                if ((InputBoxManager.InputBox("Renomer", "New document name :", ref FolderName) == DialogResult.OK))
                {
                    FileEstValide = IsValidFilename(FolderName) && FolderName.Length > 0;
                }
            }    
            try
            {
                string messageRenamed = this.monForm.managerFTP.Rename(item.Text, FolderName, monForm.sessionVariable.LastFolderSelectedDistant);
                this.monForm.consoleManager.AppendText(messageRenamed, this.monForm.consoleManager.green);
            }
            catch (Exception e)
            {
                this.monForm.consoleManager.AppendText(e.Message,this.monForm.consoleManager.red);
            }
        }

    }
}
