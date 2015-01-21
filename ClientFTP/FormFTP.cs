using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientFTP
{
    public partial class FormFTP : Form
    {
        public SessionVariable sessionVariable = new SessionVariable();
        public ManagerConsole consoleManager = new ManagerConsole();
        public managerExplorer explorerLocal = new managerExplorer();
        public managerExplorer explorerDistant = new managerExplorer();
        public ManagerFTP managerFTP;

        public Manager.ManagerProgressBar managerProgressBar;
        public FormFTP()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.managerProgressBar = new Manager.ManagerProgressBar(this);

            this.treeView1.Dock = DockStyle.Fill;
            this.treeView1.ImageList = imageList1;

            this.listView1.Dock = DockStyle.Fill;
            this.listView1.View = View.Details;

            this.treeViewDistant.Dock = DockStyle.Fill;
            this.treeViewDistant.ImageList = imageList1;

            this.listViewDistant.Dock = DockStyle.Fill;
            this.listViewDistant.View = View.Details;

            this.progressBarMain.Hide();
            this.labelProgressBar.Hide();
            this.labelProgressBarFile.Hide();

            this.richTextBox1.ReadOnly = true;
            //this.richTextBox1.Width = this.Width;

            explorerLocal.setTreeView(this.treeView1, this.listView1);
            explorerLocal.PopulateTreeView();

            explorerDistant.setTreeView(this.treeViewDistant, this.listViewDistant);


            consoleManager.setConsole(this.richTextBox1, this);
            //  consoleManager.AppendText("test", Color.Green);
            //  consoleManager.AppendText("ALEX TU GERE LA FOUGERE MEC", Color.Red);
            explorerDistant.changeBgColor(Color.Gray);
            this.buttonDisconnect.Enabled = false;



        }

        void treeView1_NodeMouseClick(object sender,
            TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();

            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;


            this.sessionVariable.LastFolderSelectedClient = nodeDirInfo.FullName.ToString();

            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                    {new ListViewItem.ListViewSubItem(item, "Directory"), 
                     new ListViewItem.ListViewSubItem(item, 
						dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"), 
                     new ListViewItem.ListViewSubItem(item, 
						file.LastAccessTime.ToShortDateString())};

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            RichTextBox myRichText = (RichTextBox)sender;
            myRichText.ScrollToCaret();
        }

        private void Launch_Click(object sender, EventArgs e)
        {

            try
            {
                this.managerFTP = new ManagerFTP(this.textBoxIP.Text, int.Parse(this.textBoxPort.Text), this.textBoxLogin.Text, this.textBoxPassword.Text,this);
                Dossier dossier = this.managerFTP.GetListFolder();
              //  dossier.path = "/";
                explorerDistant.PopulateTreeView(dossier);
                this.consoleManager.AppendText("Connected", Color.Green);
                ButtonLaunch.Enabled = false;
                explorerDistant.changeBgColor(Color.White);
                this.buttonDisconnect.Enabled = true;
            }
            catch (Exception ex)
            {
                this.consoleManager.AppendText(ex.Message, Color.Red);

                this.Reset();
            }
        }

        private void Reset()
        {
            this.ButtonLaunch.Enabled = true;
            explorerDistant.changeBgColor(Color.Gray);
            this.listViewDistant.Items.Clear();
            this.treeViewDistant.Nodes.Clear();
            this.buttonDisconnect.Enabled = false;
            this.sessionVariable.LastFolderSelectedDistant = null;

        }

        private void treeView2_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                LoadContent(e);
            }
            catch (Exception ex)
            {

                this.consoleManager.AppendText(ex.Message, Color.Red);
                this.Reset();
            }

        }

        private void LoadContent(TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listViewDistant.Items.Clear();
            Dossier dossier = (Dossier)newSelected.Tag;
            this.sessionVariable.LastFolderSelectedDistant = dossier.path;
            this.managerFTP.SetUrl(dossier.path);
            dossier = this.managerFTP.GetListFolder();
            dossier.path = (this.managerFTP.Request.RequestUri.AbsoluteUri);
            newSelected.Tag = dossier;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            bool isChildren = newSelected.Nodes.Count > 0;


            foreach (FileFromFTP file in dossier.ListFileFTP)
            {
                if (!file.isFolder)
                {
                    item = new ListViewItem(file.Name, 1);
                    subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"), 
                     new ListViewItem.ListViewSubItem(item, 
						"")};

                    item.SubItems.AddRange(subItems);
                    listViewDistant.Items.Add(item);
                    listViewDistant.Name = dossier.path;
                }
                else
                {
                    item = new ListViewItem(file.Name,0);
                    subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "Directory"), 
                     new ListViewItem.ListViewSubItem(item, 
						"")};

                    item.SubItems.AddRange(subItems);
                    listViewDistant.Items.Add(item);
                    TreeNode node = new TreeNode(file.Name);
                    Dossier nextDossier = (Dossier)dossier.Clone();
                    nextDossier.isLoaded = false;
                    nextDossier.isNodeAdded = false;
                    nextDossier.path = dossier.path + "/" + file.Name + "/";
                    node.Tag = nextDossier;
                    bool EstListe = false;
                    foreach(TreeNode oneNode in newSelected.Nodes)
                    {
                        if(oneNode.Text == node.Text)
                        {
                            EstListe = true;
                        }
                       
                    }
                    if (!EstListe)
                     newSelected.Nodes.Add(node);
                    

                    dossier.isNodeAdded = true;
                    dossier.isLoaded = true; ;
                    newSelected.Tag = dossier;
                }
            }
            this.listViewDistant.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            this.consoleManager.AppendText("Disconnected", Color.Red);

            this.managerFTP.Response.Close();
            this.managerFTP.Response.Dispose();
            this.managerFTP.Request.Abort();

            this.managerFTP.Request = null;
            this.Reset();
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            try
            {
                MouseEventArgs mouse = (MouseEventArgs)e;
                if (mouse.Button == MouseButtons.Right)
                {
                    MessageBox.Show(item.Text);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {

            MenuContextuelClient monMenu = new MenuContextuelClient(this, e, sender);
            monMenu.Show();

        }

        private void listViewDistant_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void listViewDistant_Click(object sender, EventArgs e)
        {

        }

        private void listViewDistant_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.sessionVariable.LastFolderSelectedDistant != null )
            {
                MouseEventArgs myEvent = (MouseEventArgs)e;
                MenuContextuelDistant monMenu = new MenuContextuelDistant(this, myEvent, sender);
                monMenu.Show();
            }
           
        }
    }
}
