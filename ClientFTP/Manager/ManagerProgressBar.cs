using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientFTP.Manager
{
    public class ManagerProgressBar
    {
        private FormFTP formFTP;
        public bool isActive;
        public ManagerProgressBar(FormFTP formFTP)
        {
            this.formFTP = formFTP;
            isActive = false;
        }

        public void StartProgressBar(TypeTransfert type)
        {
            MethodInvoker monInvoker = delegate
            {
                isActive = true;
                switch (type)
                {
                    case TypeTransfert.Download:
                        this.formFTP.labelProgressBar.Text = type.ToString();
                        break;
                    case TypeTransfert.Upload:
                        this.formFTP.labelProgressBar.Text = type.ToString();
                        break;
                }
                this.formFTP.labelProgressBar.Show();
                formFTP.progressBarMain.Show();
                this.SetProgression(0);
            };

            formFTP.Invoke(monInvoker);

        }

        public void SetFileName(string fileName)
        {
            MethodInvoker monInvoker = delegate
          {
              this.formFTP.labelProgressBarFile.Text = fileName;
              this.formFTP.labelProgressBarFile.Show();
              this.formFTP.Refresh();
          };
            formFTP.Invoke(monInvoker);
        }

        public void SetProgression(int progress)
        {
            MethodInvoker monInvoker = delegate
              {
                  this.formFTP.progressBarMain.Value=progress;
                  this.formFTP.labelProgressBarPercent.Text = progress + "%";
              };
            formFTP.Invoke(monInvoker);
            
   
        }


    }
}
