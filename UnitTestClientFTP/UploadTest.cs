using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClientFTP;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace UnitTestClientFTP
{
    [TestClass]
    public class UploadTest
    {
        public ManagerFTP monManagerFTP;

        [TestInitialize]
        public void initTest()
        {
            this.monManagerFTP = new ManagerFTP("127.0.0.1", 63001, "test2", "test2");
        }
        [TestMethod]
        public void UploadFile()
        {

            this.monManagerFTP.addPath("/abcd.txt");
            this.monManagerFTP.Request.Method = WebRequestMethods.Ftp.UploadFile;
            StreamReader sourceStream = new StreamReader("./testUpload.txt");
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            monManagerFTP.Request.ContentLength = fileContents.Length;
            monManagerFTP.Request.Credentials = new NetworkCredential(this.monManagerFTP.Login, this.monManagerFTP.Password);
            try
            {
                Stream requestStream = monManagerFTP.Request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
                FtpWebResponse response = (FtpWebResponse)monManagerFTP.Request.GetResponse();
                Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
                response.Close();
            }
            catch (Exception)
            {

                throw new Exception("Erreur serveur, vérifier vos autorisations.");
            }
          
             
        }

        [TestMethod]
        public void DownloadFile()
        {

            this.monManagerFTP.addPath("/abcd.txt");
            this.monManagerFTP.Request.Method = WebRequestMethods.Ftp.DownloadFile;
            monManagerFTP.Request.Credentials = new NetworkCredential(this.monManagerFTP.Login, this.monManagerFTP.Password);
            int bytesRead = 0;
            byte[] buffer = new byte[2048];

            FtpWebRequest request = monManagerFTP.Request;
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            Stream reader = request.GetResponse().GetResponseStream();
            FileStream fileStream = new FileStream("C:/Users/alexa_000/Documents/My Games/testfile2.txt", FileMode.Create);

            while (true)
            {
                bytesRead = reader.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                    break;

                fileStream.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();       


        }

    }
}
