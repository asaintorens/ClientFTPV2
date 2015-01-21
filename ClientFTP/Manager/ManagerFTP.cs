using ClientFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Security.Principal;
using System.Threading;
namespace ClientFTP
{
    public class ManagerFTP
    {
        private string AdresseIp;
        private int Port;
        public string Login;
        public string Password;
        public FtpWebRequest Request;
        public FtpWebResponse Response;
        private string URL;
        public FormFTP formFTP;
        public string url
        {
            get
            {
                return URL;
            }
            set
            {
                this.URL = GetUrl() + value;
                this.Request = (FtpWebRequest)WebRequest.Create(this.URL);
            }
        }


        public string GetUrl()
        {
            return "ftp://" + this.AdresseIp + ":" + this.Port;
        }
        public ManagerFTP(string ip, int port, string login, string password,FormFTP formFTP)
        {
            // TODO: Complete member initialization
            this.formFTP = formFTP;
            this.URL = "";
            this.AdresseIp = ip;
            this.Port = port;
            this.Login = login;
            this.Password = password;
            //this.Request = (FtpWebRequest)WebRequest.Create(this.url);
            this.Request = (FtpWebRequest)WebRequest.Create(this.GetUrl());


            SetCredential();
        }

        private void SetCredential()
        {
            Request.Credentials = new NetworkCredential(this.Login, this.Password);
        }


        public void addPath(string path)
        {
            if (path.ElementAt(0).ToString() != "/")
            {
                this.url += "/" + path;
            }
            else
                this.url += path;
        }

        /// <summary>
        /// envoi d'un ping vers le server FTP
        /// </summary>
        /// <returns>true si réponse sinon false</returns>
        public bool Communiquer()
        {
            bool communication = false;
            try
            {
                TcpClient client = new TcpClient(this.AdresseIp, this.Port);
                communication = true;
            }
            catch (Exception ex)
            {
                communication = false;
            }

            return communication;
        }

        public void RequestServer()
        {
            try
            {
                Request.Credentials = new NetworkCredential(this.Login, this.Password);
                this.Response = (FtpWebResponse)Request.GetResponse();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dossier GetListFolder()
        {
            this.Request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            this.RequestServer();
            Stream responseStream = this.Response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            List<string> directories = new List<string>();

            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                directories.Add(line);
                line = reader.ReadLine();
            }
            reader.Close();
            responseStream.Close();
            return this.GenererDossier(directories);

        }

        /// <summary>
        /// Genere le Dossier avec une liste de string
        /// </summary>
        /// <param name="directories"></param>
        private Dossier GenererDossier(List<string> directories)
        {
            Dossier DossierRoot = new Dossier();
            DossierRoot.path = this.url;
            FileFromFTP oneFile;
            foreach (string oneStringFile in directories)
            {
                DossierRoot.Add(new FileFromFTP(oneStringFile));
            }
            DossierRoot.isLoaded = true;


            for (int indexFile = 0; indexFile < DossierRoot.ListFileFTP.Count(); indexFile++)
            {
                if (DossierRoot.ListFileFTP.ElementAt(indexFile).Name == "." || DossierRoot.ListFileFTP.ElementAt(indexFile).Name == "..")
                {
                    DossierRoot.ListFileFTP.Remove(DossierRoot.ListFileFTP.ElementAt(indexFile));
                    indexFile--;
                }
            }


            return DossierRoot;
        }



        public void GetSubdirectory()
        {
            this.Request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            this.RequestServer();
            StreamReader remoteDirContents = new StreamReader(this.Response.GetResponseStream());

            if (remoteDirContents == null)
            {
                //
                // Add appropriate error handling here, and exit out 
                // of your function as needed if we can't read the FTP
                // Request's response stream...
                //
            }

            bool finished = false;
            string directoryData = string.Empty;
            StringBuilder remoteFiles = new StringBuilder();

            do
            {
                directoryData = remoteDirContents.ReadLine();

                if (directoryData != null)
                {
                    if (remoteFiles.Length > 0)
                    {
                        remoteFiles.Append("\n");
                    }

                    remoteFiles.AppendFormat("{0}", directoryData);
                }

                else
                {
                    finished = true;
                }
            }
            while (!finished);
        }

        public void SetUrl(string newUrl)
        {


            bool isLongUrl = false;
            if (newUrl.Length != 0)
                if (newUrl.ElementAt(0).ToString() == "f")
                    if (newUrl.ElementAt(1).ToString() == "t")
                        if (newUrl.ElementAt(2).ToString() == "p")
                            if (newUrl.ElementAt(3).ToString() == ":")
                                isLongUrl = true;

            if (isLongUrl)
            {
                this.Request = (FtpWebRequest)WebRequest.Create(newUrl);
            }
            else
            {
                this.Request = (FtpWebRequest)WebRequest.Create(this.GetUrl() + "/" + newUrl);
            }


        }

        public string Uploader(string pathFileToUpload, string fileName, string pathOnServer)
        {
            

            string message = "";
            // MessageBox.Show(WindowsIdentity.GetCurrent().Name);

            this.SetUrl(pathOnServer + "/" + fileName);
            this.Request.Method = WebRequestMethods.Ftp.UploadFile;
            StreamReader sourceStream = null;
            try
            {
                var fs = new FileStream(pathFileToUpload + "/" + fileName, FileMode.Open, FileAccess.ReadWrite);
                sourceStream = new StreamReader(fs);

            }
            catch (Exception ex)
            {

                throw ex;

            }

            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            Request.ContentLength = fileContents.Length;
            SetCredential();
            try
            {
                Stream requestStream = Request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)Request.GetResponse();

                Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
                message = response.StatusDescription;
                response.Close();
            }
            catch (Exception)
            {

                throw new Exception("Erreur serveur, vérifier vos droits.");
            }
            return message;
        }

        public string CreerDossier(string pathFileToUpload, string fileName, string pathOnServer)
        {
            string message = "";
            try
            {
                this.SetUrl(pathOnServer + "/" + fileName);
                this.Request.Method = WebRequestMethods.Ftp.MakeDirectory;
                this.SetCredential();
                using (var resp = (FtpWebResponse)this.Request.GetResponse())
                {
                    message = resp.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
           

            return message;
        }

        public string Download(string PathLocal, string FileName, string pathServer)
        {
            string message = "";
            this.formFTP.managerProgressBar.SetFileName(FileName);
            
           // Thread.Sleep(1000);
            this.SetUrl(pathServer + "/" + FileName);
            Request.Method = WebRequestMethods.Ftp.GetFileSize;
            this.SetCredential();
            long fileSize; // this is the key for ReportProgress
            using (WebResponse resp = this.Request.GetResponse())
            {

                fileSize = resp.ContentLength;
                resp.Close();
            }
            this.SetUrl(pathServer + "/" + FileName);
            this.SetCredential();
            this.Request.Method = WebRequestMethods.Ftp.DownloadFile;
           
            int bytesRead = 0;
            int totalBytesRead = 0;
            byte[] buffer = new byte[2048];
             using (var resp = (FtpWebResponse)this.Request.GetResponse())
             {
                 Stream reader = resp.GetResponseStream();
                 FileStream fileStream = new FileStream(PathLocal + "/" + FileName, FileMode.Create);
               long ln =  resp.ContentLength;
                 while (true)
                 {
                     bytesRead = reader.Read(buffer, 0, buffer.Length);
                     totalBytesRead += bytesRead;
                     if (bytesRead == 0)
                         break;
                     fileStream.Write(buffer, 0, bytesRead);
                     double percent = ((double)totalBytesRead /(double) fileSize) * 100;
                     //long percent =( (totalBytesRead/1024)*100)  / (fileSize/1024);
                     this.formFTP.managerProgressBar.SetProgression((int)Math.Round(percent, 2));
                 }
                 message = resp.StatusDescription;
                 fileStream.Close();       
             }
           

            return message;
        }

        public string Rename(string fileName,string futureFileName,string pathServer)
        {
            string message = "";
                this.SetUrl(pathServer + fileName);
                Request.Method = WebRequestMethods.Ftp.Rename;
                this.SetCredential();

                Request.RenameTo = futureFileName + GetExtension(fileName) ;
                using (var resp = (FtpWebResponse)this.Request.GetResponse())
                {
                    message = resp.StatusDescription;
                }


            return message;
        }

        private string GetExtension(string fileName)
        {
            String Message = "";
            String[] extension = fileName.Split('.');
            if (fileName.Contains('.'))
            {
                Message ="."+ extension[extension.Length - 1];
            }
            return Message;
        }

        public string DeleteFile(string fileName, string pathServer)
        {
            string message = "";
            this.SetUrl(pathServer + "/" + fileName);
            Request.Method = WebRequestMethods.Ftp.DeleteFile;

            this.SetCredential();
            using (var resp = (FtpWebResponse)this.Request.GetResponse())
            {
                message = resp.StatusDescription;
            }


            return message;
        }

        public string DeleteFolder(string fileName, string pathServer)
        {
            string message = "";
            this.SetUrl(pathServer + "/" + fileName);
            Request.Method = WebRequestMethods.Ftp.RemoveDirectory;

            this.SetCredential();
            using (var resp = (FtpWebResponse)this.Request.GetResponse())
            {
                message = resp.StatusDescription;
            }


            return message;
        }
    }
}
