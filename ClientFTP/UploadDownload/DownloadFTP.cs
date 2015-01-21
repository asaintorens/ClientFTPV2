using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientFTP.UploadDownload
{
    public class DownloadFTP
    {
        private FormFTP form1;

        public DownloadFTP(FormFTP form1)
        {
            // TODO: Complete member initialization
            this.form1 = form1;
        }

        public void Download(string FileName)
        {
            Thread threadDownload = new Thread(() => this.ThreadDownloading(FileName));
            threadDownload.Start();

           // ThreadDownloading(FileName);
        }

        public void ThreadDownloading(string FileName)
        {
            this.form1.managerProgressBar.StartProgressBar(Manager.TypeTransfert.Download);
            this.form1.managerProgressBar.SetFileName(FileName);
            try
            {
                string res = form1.managerFTP.Download(form1.sessionVariable.LastFolderSelectedClient, FileName, form1.sessionVariable.LastFolderSelectedDistant);
                this.form1.consoleManager.AppendText(res, System.Drawing.Color.Green);
            }
            catch (Exception ex)
            {

                this.form1.consoleManager.AppendText(ex.Message, System.Drawing.Color.Red);
            }
            this.form1.managerProgressBar.SetProgression(100);
            
        }
    }
}
