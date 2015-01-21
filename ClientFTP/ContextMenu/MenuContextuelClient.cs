using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientFTP
{

    public class MenuContextuelClient
    {
        ContextMenuStrip ContextMenu = new ContextMenuStrip();

        FormFTP monForm;
        MouseEventArgs monEvent;
        object monSender;

        public MenuContextuelClient(FormFTP leForm, MouseEventArgs leEvent, object lesender)
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
                ContextMenu.Show(item, mouse.Location);
            }


        }

        private void SetEvent()
        {
            ContextMenu.ItemClicked += new ToolStripItemClickedEventHandler(this.contextMenu_ItemClicked);
        }

        private void ChargerAction()
        {
            foreach (string uneAction in ContextMenuActionClient.GetNames(typeof(ContextMenuActionClient)))
            {
                ContextMenu.Items.Add(uneAction);
            }
        }
        private void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            if (item.Text == ContextMenuActionClient.SelectAll.ToString())
            {
                foreach (ListViewItem oneListViewItem in monForm.listView1.Items)
                {
                    oneListViewItem.Selected = true;
                }
            }
            if (item.Text == ContextMenuActionClient.Uploader.ToString())
            {
                Upload.UploadFTP upload = new Upload.UploadFTP(monForm);
                //Console.WriteLine(monForm.sessionVariable.LastFolderSelected);
                upload.UpLoadFile();
            }

            // your code here
        }
    }
}
