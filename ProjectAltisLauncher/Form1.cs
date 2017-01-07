﻿using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
/// <summary>
/// TODO:
///     Improve Buttons Look
///     Add changelog
///     Add group tracker
///     Add invasion tracker
/// </summary>
namespace ProjectAltisLauncher
{
    public partial class Form1 : Form
    {
        #region Main Form Events
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                txtUser.Text = Properties.Settings.Default.username;
                txtPass.Text = Properties.Settings.Default.password;
            }
            catch { }
            // Read user settings
            if (Properties.Settings.Default.wantsCursor == true) // Cursor
            {
                MemoryStream cursorMemoryStream = new MemoryStream(Properties.Resources.toonmono);
                this.Cursor = new Cursor(cursorMemoryStream);
            }
            // Load last saved user background choice
            SetBackground(Properties.Settings.Default.background);
            new System.Threading.Thread(() =>
            {
                CheckForUpdate();
            }).Start(); 
            // This prevents other controls from being focused
            this.Select();
            this.ActiveControl = null;
        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }
        private void Form1_Deactivate(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }
        #endregion
        #region Global Variables
        private string currentDir = Directory.GetCurrentDirectory() + "\\";
        private double totalFiles = 0;
        private double totalProgress;
        private double currentFile = 0;
        private string nowDownloading = "";
        #endregion
        #region Borderless Form Code
        Point mouseDownPoint = Point.Empty;


        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownPoint = new Point(e.X, e.Y);
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDownPoint = Point.Empty;
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDownPoint.IsEmpty)
                return;
            Form f = sender as Form;
            f.Location = new Point(f.Location.X + (e.X - mouseDownPoint.X), f.Location.Y + (e.Y - mouseDownPoint.Y));
        }
        #endregion
        #region Button Behaviors
        #region Exit Button
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnExit_MouseEnter(object sender, EventArgs e)
        {
            btnExit.BackgroundImage = Properties.Resources.cancel_h;
        }
        private void btnExit_MouseLeave(object sender, EventArgs e)
        {
            btnExit.BackgroundImage = Properties.Resources.cancel;
        }
        #endregion
        #region Minimize Button
        private void btnMin_Click(object sender, EventArgs e)
        {
            PlaySoundFile("sndclick");
            this.WindowState = FormWindowState.Minimized;
            this.ActiveControl = null;
        }
        private void btnMin_MouseEnter(object sender, EventArgs e)
        {
            btnMin.BackgroundImage = Properties.Resources.minus_h;
        }
        private void btnMin_MouseLeave(object sender, EventArgs e)
        {
            btnMin.BackgroundImage = Properties.Resources.minus;
        }
        #endregion
        #region Play Button
        private void btnPlay_Click(object sender, EventArgs e)
        {
            btnPlay.Enabled = false;
            PlaySoundFile("sndclick");
            if (cbSaveLogin.Checked == true)
            {
                Console.WriteLine("Save checked");
                if (txtUser.Text != null || txtPass.Text != null)
                {
                    Properties.Settings.Default.username = txtUser.Text;
                    Properties.Settings.Default.password = txtPass.Text;
                    Properties.Settings.Default.Save();
                }

            }
            string finalURL = "https://www.projectaltis.com/api/?u=" + txtUser.Text + "&p=" + txtPass.Text;
            string APIResponse = RequestData(finalURL, "GET"); // Send request to login API, store the response as string
            loginAPIResponse resp = JsonConvert.DeserializeObject<loginAPIResponse>(APIResponse);

            switch (resp.status)
            {
                case "true":
                    lblInfo.Text = resp.reason;
                    Updater.RunWorkerAsync();
                    break;
                case "false":
                    lblInfo.Text = resp.reason;
                    btnPlay.Enabled = true;
                    break;
                case "critical":
                    lblInfo.Text = resp.additional;
                    btnPlay.Enabled = true;
                    break;
                case "info":
                    lblInfo.Text = resp.reason;
                    btnPlay.Enabled = true;
                    break;
            }
            lblInfo.Visible = true;
            this.ActiveControl = null;
        }
        #endregion
        private void btnOfficialSite_Click(object sender, EventArgs e)
        {
            PlaySoundFile("sndclick");
            Process.Start("https://www.projectaltis.com/");
            this.ActiveControl = null;
        }
        private void btnDiscord_Click(object sender, EventArgs e)
        {
            PlaySoundFile("sndclick");
            Process.Start("https://discord.gg/szEPYtV");
            this.ActiveControl = null;
        }
        private void btnGroupTracker_Click(object sender, EventArgs e)
        {
            PlaySoundFile("sndclick");
            MessageBox.Show("Group Tracker will be implemented soon!", "Oops!");
            this.ActiveControl = null;
        }
        private void btnCredits_Click(object sender, EventArgs e)
        {
            PlaySoundFile("sndclick");
            Credits f = new Credits();
            f.ShowDialog();
            this.ActiveControl = null;
        }
        private void btnChangeBg_Click(object sender, EventArgs e)
        {
            PlaySoundFile("sndclick");
            BackgroundChoices bg = new BackgroundChoices();
            bg.ShowDialog();
            SetBackground(Properties.Settings.Default.background);
            this.ActiveControl = null;
        }
        private void btnOptions_Click(object sender, EventArgs e)
        {
            PlaySoundFile("sndclick");
            Options op = new Options();
            op.ShowDialog();
            // Apply user settings
            if (Properties.Settings.Default.wantsCursor == true) // Cursor
            {
                MemoryStream cursorMemoryStream = new MemoryStream(Properties.Resources.toonmono);
                this.Cursor = new Cursor(cursorMemoryStream);
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
            this.ActiveControl = null;
        }
        #endregion
        #region Hashing Functions
        private bool CompareFileSize(string filePath, int size)
        {
            try
            {
                FileInfo myFile = new FileInfo(filePath);
                long sizeInBytes = myFile.Length;
                return sizeInBytes == size; // returns true or false

            }
            catch (Exception)
            {
                // The file doesn't exist
                Console.WriteLine("{0} does not exist!", filePath);
                return false;
            }
        }
        private bool CompareSHA256(string filePath, string hash)
        {
            try
            {
                SHA256 mySHA256 = SHA256.Create();

                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    byte[] hashValue = mySHA256.ComputeHash(fileStream);
                    string strHashValue = "";
                    foreach (byte x in hashValue)
                    {
                        strHashValue += x.ToString("x2");
                    }
                    // Comparing the hash now
                    Console.WriteLine("The SHA256 of {0} is: {1}", filePath, strHashValue);

                    return strHashValue == hash;
                }

            }
            catch (Exception)
            {
                // File doesn't exist
                Console.WriteLine("{0} does not exist!", filePath);
                return false;
            }
        }
        #endregion
        private static void PlaySoundFile(string filename)
        {
            System.Media.SoundPlayer player;
            switch (filename.ToLower())
            {
                case "sndclick":
                    player = new System.Media.SoundPlayer(Properties.Resources.sndclick);
                    player.Load();
                    player.Play();
                    break;
            }

        }
        private string RequestData(string URL, string Method)
        {
            try
            {
                WebRequest request = WebRequest.Create(URL);
                request.Method = Method;
                WebResponse response = request.GetResponse();
                Stream dataStream = default(Stream);
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                return responseFromServer;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Thrown: " + "\n  Type:    " + ex.GetType().Name + "\n  Message: " + ex.Message);
                this.Close();
            }
            return "";
        }
        private void Updater_DoWork(object sender, DoWorkEventArgs e)
        {
            currentFile = 0; // Reset the value so every time user plays totalProg
            string responseFromServer = RequestData("https://www.projectaltis.com/api/manifest", "GET");
            string[] array = responseFromServer.Split('#'); // Seperate each json value into an index


            Console.WriteLine("The length of array is {0}", array.Length);
            Directory.CreateDirectory(currentDir + "config\\");
            Directory.CreateDirectory(currentDir + "resources\\");
            Directory.CreateDirectory(currentDir + "resources\\default\\");
            totalFiles = array.Length - 1;
            for (int i = 0; i < array.Length - 1; i++)
            {
                currentFile += 1;
                manifest patchManifest = JsonConvert.DeserializeObject<manifest>(array[i]);
                WebClient client = new WebClient();
                if (patchManifest.filename != null || patchManifest.filename != "")
                {
                    if (patchManifest.filename.Contains("phase"))
                    {
                        if (CompareFileSize(currentDir + "resources\\default\\" + patchManifest.filename, Convert.ToInt32(patchManifest.size)))
                        {
                            Console.WriteLine("Phase file: {0} is up to date!", patchManifest.filename);
                        }
                        else
                        {
                            nowDownloading = patchManifest.filename;
                            Updater.ReportProgress(0); // Fire the progress changed event
                            Console.WriteLine("Starting download for phase file: {0}", patchManifest.filename);
                            client.DownloadFile(new Uri(patchManifest.url), currentDir + "resources\\default\\" + patchManifest.filename);
                            Console.WriteLine("Finished!");
                        }
                    }
                    else if (patchManifest.filename.Contains("ProjectAltis"))
                    {
                        if (CompareSHA256(currentDir + "config\\" + patchManifest.filename, patchManifest.sha256))
                        {

                        }
                        else
                        {
                            nowDownloading = patchManifest.filename;
                            Updater.ReportProgress(0); // Fire the progress changed event
                            client.DownloadFile(new Uri(patchManifest.url), currentDir + "config\\" + patchManifest.filename);
                        }

                    }
                    else if (CompareSHA256(currentDir + patchManifest.filename, patchManifest.sha256)) // If the hashes are the same skip the update
                    {
                        Console.WriteLine("{0} is up to date!", patchManifest.filename);
                    }
                    else
                    {
                        nowDownloading = patchManifest.filename;
                        Updater.ReportProgress(0); // Fire the progress changed event
                        Console.WriteLine("Starting download for file: {0}", patchManifest.filename);
                        client.DownloadFile(new Uri(patchManifest.url), currentDir + patchManifest.filename);
                        Console.WriteLine("Finished!");
                    }
                }



                totalProgress = ((currentFile / totalFiles) * 100);
                Console.WriteLine("Total progress is {0}", totalProgress);
                Updater.ReportProgress(Convert.ToInt32(totalProgress));

            }
        }
        private void Updater_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblNowDownloading.Visible = true;
            lblNowDownloading.Text = "Downloading " + nowDownloading;
            if (totalProgress == 100)
            {
                btnPlay.Enabled = true;
                lblNowDownloading.Text = "";
                lblNowDownloading.Visible = false;
                // Launch game once all files are download
                Play.LaunchGame(txtUser.Text, txtPass.Text);
            }

        }
        private void SetBackground(string bg)
        {
            switch (bg)
            {
                case "TTC":
                    BackgroundImage = Properties.Resources.TTC;
                    break;
                case "DD":
                    BackgroundImage = Properties.Resources.DD;
                    break;
                case "DG":
                    BackgroundImage = Properties.Resources.DG;
                    break;
                case "MML":
                    BackgroundImage = Properties.Resources.MML;
                    break;
                case "Brrrgh":
                    BackgroundImage = Properties.Resources.Brrrgh;
                    break;
                case "DDL":
                    BackgroundImage = Properties.Resources.DDL;
                    break;
            }
        }
        /// <summary>
        /// Checks for the latest update of the launcher
        /// </summary>
        private void CheckForUpdate()
        {
            string responseFromServer = RequestData("https://www.projectaltis.com/api/launcherManifest", "GET");
            string[] array = responseFromServer.Split('#'); // Seperate each json value into an index

            for(int i = 0; i < array.Length - 1; i++) // - 1 Because string split creates one extra null line D:
            {
                // First compare the file against current file and see if it needs to be updated
                manifest patchManifest = JsonConvert.DeserializeObject<manifest>(array[i]);

                WebClient client = new WebClient();
                if (CompareSHA256(currentDir + patchManifest.filename, patchManifest.sha256)) { return; }

                if (patchManifest.filename.ToLower() != "launcher.exe")
                {
                    client.DownloadFile(patchManifest.url, patchManifest.filename);
                    return;
                }
                    client.DownloadFile(patchManifest.url, "Launcher_New.exe");
                    File.Delete(Path.GetTempPath() + @"\script.vbs");
                    using (StreamWriter file = new StreamWriter(Path.GetTempPath() + @"\updater.vbs", true))
                    {
                    file.WriteLine("Dim f");
                    file.WriteLine("Set f = WScript.CreateObject(\"Scripting.FileSystemObject\")");
                    file.WriteLine("Set obj = CreateObject(\"Scripting.FileSystemObject\")");
                    file.WriteLine("obj.DeleteFile(\"{0}\")", currentDir + "Launcher.exe"); // Deletes current launcher to prevent IO Errors
                    file.WriteLine("f.MoveFile " + "\"" + currentDir + "Launcher_New.exe" +"\", " + "\"" + currentDir + "Launcher.exe" + "\"");
                    file.WriteLine("WSHShell.Run(\"{0}\")", currentDir + "Launcher.exe");
                        Process.Start(Path.GetTempPath() + @"\updater.vbs");
                        Application.Exit();                   
                }

  }
        }
    }
}
