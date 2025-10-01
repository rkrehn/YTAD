using Fastenshtein;
using Microsoft.VisualBasic;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using TagLib;
using YTPD.Properties;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using File = System.IO.File;

namespace YTPD
{
    public partial class Form1 : Form
    {
        public double prog = 0;
        public static bool isConverting = false;
        public static bool isPaused = false;
        public static bool isSaving = false;
        public string albumURL = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // set up saved data
            txt_Dir.Text = Properties.Settings.Default.Directory;
            txt_Cookies.Text = Properties.Settings.Default.CookieFile;
            chk_Cookies.Checked = Properties.Settings.Default.UseCookies;
            LoadCSVIntoDataGridView();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            GetAlbumInfo2Async(txt_URL.Text);
            //GetAlbumData(txt_URL.Text);
        }

        //private async Task GetAlbumData(string theURL)
        //{
        //    btn_GetAlbum.Enabled = false;
        //    var youtube = new YoutubeClient();
        //    string album = "";
        //    string thumb = "";

        //    // You can specify either the video URL or its ID
        //    try
        //    {
        //        var albuminfo = await youtube.Playlists.GetAsync(theURL);
        //        album = albuminfo.Title;
        //        thumb = albuminfo.Thumbnails[1].Url;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteError(ex.ToString());
        //        MessageBox.Show("Unable to add album. Please copy/paste again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        btn_GetAlbum.Enabled = true;
        //        return;
        //    }

        //    // Sometimes YouTube adds "Album - " or "- Topic" to the front/back of the album name, so let's clean that up
        //    album = album.Replace("Album - ", "");
        //    album = album.Replace("- Topic", "");

        //    // Get all playlist videos
        //    Int16 songnum = 1;

        //    try
        //    {
        //        // Basically a foreach loop, but async and add a row for each song
        //        await foreach (var video in youtube.Playlists.GetVideosAsync(theURL))
        //        {
        //            var title = video.Title;
        //            var band = video.Author.ChannelTitle;
        //            var author = video.Author;
        //            var duration = video.Duration;
        //            var link = video.Url;

        //            band = band.Replace("- Topic", "").Trim();

        //            dgv_downloads.Rows.Add(band, album, songnum.ToString(), title, duration, link, "0", "0", "No");
        //            songnum++;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteError(ex.ToString());
        //        MessageBox.Show("Unable to grab playlist: \r" + ex.ToString());
        //    }

        //    // refresh and clear for next entry
        //    dgv_downloads.Refresh();
        //    txt_URL.Clear();
        //    txt_URL.Focus();

        //    // saving is good
        //    SaveDataGridViewToCSV();
        //    btn_GetAlbum.Enabled = true;
        //}

        private async void timer1_Tick(object sender, EventArgs e)
        {
            // ensure a directory exists
            if (!Directory.Exists(txt_Dir.Text))
            {
                MessageBox.Show("Directory does not exist! Please browse for a new directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                timer1.Enabled = false;
                return;
            }

            // turn it off while it's running
            timer1.Enabled = false;

            // clear up variables
            string artist = "";
            string album = "";
            string songnum = "";
            string song = "";
            string duration = "";
            string link = "";

            foreach (DataGridViewRow row in dgv_downloads.Rows)
            {
                // if the user paused downloading, then go away
                if (isPaused) return;

                // delete empty rows
                if (row.Cells[0].Value == null || row.Cells[0].Value.ToString().Length < 1)
                {
                    try
                    {
                        dgv_downloads.Rows.RemoveAt(row.Index);
                    }
                    catch (Exception ex)
                    {
                        WriteError(ex.ToString());
                        break;
                    }

                    continue;
                }

                // converting download percentage to an integer
                Int16 dlpercent = Convert.ToInt16(row.Cells["DL"].Value);

                // if it's between 0 and 100, then it's already downloading, so skip it
                if (dlpercent > 0 && dlpercent < 100) return;

                // if the next item hasn't been downloaded yet, then process it
                if (dlpercent == 0)
                {
                    // prepare variables
                    artist = row.Cells["Artist"].Value.ToString();
                    album = row.Cells["Album"].Value.ToString();
                    songnum = row.Cells["SongNum"].Value.ToString();
                    song = row.Cells["Song"].Value.ToString();
                    duration = row.Cells["Duration"].Value.ToString();
                    link = row.Cells["Link"].Value.ToString();
                    link = System.Text.RegularExpressions.Regex.Replace(link, @"&list=[^&]*", "");

                    row.Cells["DL"].Value = "1";

                    // create artist directory
                    if (!Directory.Exists(txt_Dir.Text + "\\" + artist))
                    {
                        Directory.CreateDirectory(txt_Dir.Text + "\\" + GetValidFilename(artist));
                    }

                    // create album directory
                    if (!Directory.Exists(txt_Dir.Text + "\\" + artist + "\\" + album))
                    {
                        Directory.CreateDirectory(txt_Dir.Text + "\\" + GetValidFilename(artist) + "\\" + GetValidFilename(album));
                    }

                    // highlight the row being used
                    dgv_downloads.Rows[row.Index].DefaultCellStyle.BackColor = Color.SteelBlue;
                    dgv_downloads.Rows[row.Index].DefaultCellStyle.ForeColor = Color.Azure;
                    dgv_downloads.FirstDisplayedScrollingRowIndex = row.Index;

                    // get the manifest information
                    string fullpath = "";
                    try
                    {
                        // old code here
                        //var youtube = new YoutubeClient();
                        //var streamManifest = await youtube.Videos.Streams.GetManifestAsync(link);
                        //var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                        // set the path
                        //fullpath = Path.Combine(txt_Dir.Text + "\\" + GetValidFilename(artist) + "\\" + GetValidFilename(album), songnum + " - " + GetValidFilename(song) + "." + streamInfo.Container);
                        fullpath = Path.Combine(txt_Dir.Text + "\\" + GetValidFilename(artist) + "\\" + GetValidFilename(album), songnum + " - " + GetValidFilename(song) + ".mp3");

                        // does the file already exist? If so, set it as downloaded and move on
                        if (System.IO.File.Exists(fullpath))
                        {
                            row.Cells["DL"].Value = "100";
                            dgv_downloads.Rows[row.Index].DefaultCellStyle.BackColor = Color.Azure;
                            dgv_downloads.Rows[row.Index].DefaultCellStyle.ForeColor = Color.Black;
                            break;
                        }

                        string strargument;
                        if (chk_Cookies.Checked == true)
                        {
                            // ensure cookie file exists
                            if (!File.Exists(txt_Cookies.Text))
                            {
                                PauseSystem();
                                MessageBox.Show("Cookie file does not exist! Please select a valid cookie file or uncheck 'Use Cookies'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            // use cookies
                            strargument = $"-o \"{fullpath}\" -i -t mp3 --cookies {txt_Cookies.Text} \"{link}\"";
                        }
                        else
                        {
                            // no cookies
                            strargument = $"-o \"{fullpath}\" -i -t mp3 \"{link}\"";
                        }

                        // setup process info
                        var processInfo = new ProcessStartInfo
                        {
                            FileName = Path.Combine(Application.StartupPath, "yt-dlp.exe"),
                            Arguments = strargument,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            RedirectStandardInput = false
                        };

                        using var process2 = Process.Start(processInfo);

                        // Simple regex to catch download percentage
                        var progressRegex = new System.Text.RegularExpressions.Regex(@"\[download\]\s+(\d+\.?\d*)%");

                        // Handle output line by line as it comes in
                        string allOutput = "";
                        string errorBuilder = "";
                        process2.OutputDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                allOutput += e.Data + "\n";
                                var match = progressRegex.Match(e.Data);
                                if (match.Success)
                                {
                                    var percentage = match.Groups[1].Value;
                                    // Update UI thread-safe
                                    dgv_downloads.Invoke(new Action(() =>
                                    {
                                        row.Cells["DL"].Value = Math.Round(Convert.ToDecimal(percentage), 0);
                                        if (Convert.ToDecimal(percentage) == 100) row.Cells["Converted"].Value = "Converting...";
                                    }));
                                }
                            }
                        };

                        // handle error by line as it comes in
                        process2.ErrorDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                errorBuilder += e.Data + "\n";
                            }
                        };

                        // Start reading output
                        process2.BeginOutputReadLine();
                        process2.BeginErrorReadLine();

                        // Set initial status
                        row.Cells["DL"].Value = 1;

                        await process2.WaitForExitAsync();

                        // Final status update (same as your existing code)
                        if (process2.ExitCode == 0)
                        {
                            row.Cells["DL"].Value = "100";
                            dgv_downloads.Rows[row.Index].DefaultCellStyle.BackColor = Color.Azure;
                            dgv_downloads.Rows[row.Index].DefaultCellStyle.ForeColor = Color.Black;
                        }
                        else
                        {
                            dgv_downloads.Rows[row.Index].DefaultCellStyle.BackColor = Color.DarkRed;
                            dgv_downloads.Rows[row.Index].DefaultCellStyle.ForeColor = Color.White;
                            WriteError(allOutput + "\n" + errorBuilder);
                            if (errorBuilder.Contains("cookies"))
                            {
                                row.Cells["DL"].Value = "0";
                                PauseSystem();
                                MessageBox.Show("YouTube is requiring cookies to confirm your age and/or this not being a bot. Unfortunately, we have to pause downloads.", "YouTube Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Form frm = new frmCookies();
                                frm.ShowDialog();
                            }
                            if (errorBuilder.Contains("unavailable"))
                            {
                                row.Cells["DL"].Value = "100";
                                PauseSystem();
                                MessageBox.Show("YouTube is showing this song as unavailable for download. We'll have to skip for now.", "YouTube Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Form frm = new frmCookies();
                                frm.ShowDialog();
                            }
                        }

                        //var youTube = YouTube.Default; // starting point for YouTube actions
                        //var video = youTube.GetVideo(link); // gets a Video object with info about the video
                        //fullpath += video.FileExtension;
                        //File.WriteAllBytes(fullpath, video.GetBytes());

                        // actual stream
                        //var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
                        //await youtube.Videos.Streams.DownloadAsync(streamInfo, fullpath, progress);

                        // set colors
                        dgv_downloads.Rows[row.Index].DefaultCellStyle.BackColor = Color.Azure;
                        dgv_downloads.Rows[row.Index].DefaultCellStyle.ForeColor = Color.Black;
                    }
                    catch (Exception ex)
                    {
                        // things broke here
                        WriteError(ex.ToString());
                        row.Cells["DL"].Value = "100";
                        dgv_downloads.Rows[row.Index].DefaultCellStyle.BackColor = Color.DarkRed;
                        dgv_downloads.Rows[row.Index].DefaultCellStyle.ForeColor = Color.White;

                    }
                    //finally
                    //{
                    //    // time to conver it to an mp3 if it's not already one
                    //    if (!isConverting && fullpath.Length > 1)
                    //    {
                    //        isConverting = true;
                    //        await ConvertFile(fullpath, fullpath.Substring(fullpath.LastIndexOf('.')));
                    //        Console.WriteLine(fullpath);
                    //        isConverting = false;
                    //    }
                    //}

                    // saving is good
                    SaveDataGridViewToCSV();
                    break;
                }
            }

            // turn timer back on when done
            timer1.Enabled = true;
        }

        static async Task ConvertFile(string inputFilePath, string fileExt)
        {
            // setup variables and output path 
            string outputFilePath = inputFilePath.Replace(fileExt, ".mp3");
            string ffmpegcom = $"-n -i \"{inputFilePath}\" \"{outputFilePath}\"";

            // wait for the file to be free
            int RetryCount = 0;
            int MaxRetries = 60;

            // this will force the conversion to give up after 60 seconds of trying
            while (RetryCount < MaxRetries)
            {
                try

                {
                    using (FileStream s = System.IO.File.Open(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        s.Close();
                        break;
                    }
                }
                catch (IOException ex)
                {
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }

            }

            // Setup parameters
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = "ffmpeg",
                Arguments = ffmpegcom,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            // Start the process
            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();

                // Wait for the process to exit
                await Task.Run(() => process.WaitForExit());

                // Check the exit code
                int exitCode = process.ExitCode;

                // Handle the result based on the exit code
                if (exitCode == 0)
                {
                    Console.WriteLine("Conversion completed successfully.");
                    await Task.Run(() => System.IO.File.Delete(inputFilePath));
                }
                else
                {
                    Console.WriteLine($"Error: {exitCode}");
                }
            }
        }

        private void timer_tag_Tick(object sender, EventArgs e)
        {
            // prepare variables
            string[] artist;
            string album = "";
            string songnum = "";
            string song = "";
            string duration = "";
            string link = "";

            // this will go ahead and tag each mp3 file so mp3 players will be able to identify the artist/album/song info
            foreach (DataGridViewRow row in dgv_downloads.Rows)
            {
                if (row.Cells[0].Value == null || row.Cells[0].Value == "") return;

                Int16 dlpercent = Convert.ToInt16(row.Cells["DL"].Value);
                if (dlpercent == 100 && row.Cells["Tagged"].Value.ToString() == "0")
                {
                    row.Cells["Converted"].Value = "Tagging...";
                    artist = new[] { row.Cells["Artist"].Value.ToString() };
                    album = row.Cells["Album"].Value.ToString();
                    songnum = row.Cells["SongNum"].Value.ToString();
                    song = row.Cells["Song"].Value.ToString();
                    duration = row.Cells["Duration"].Value.ToString();
                    link = row.Cells["Link"].Value.ToString();

                    // actual stream
                    string fullpath = Path.Combine(txt_Dir.Text + "\\" + GetValidFilename(row.Cells["Artist"].Value.ToString()) + "\\" + GetValidFilename(album), songnum + " - " + GetValidFilename(song) + ".mp3");

                    //remove webm file if it exists
                    if (File.Exists(fullpath) && File.Exists(fullpath.Replace(".mp3", ".webm")))
                    {
                        try
                        {
                            File.Delete(fullpath.Replace(".mp3", ".webm"));
                            row.Cells["Converted"].Value = "Yes";
                        }
                        catch (Exception ex)
                        {
                            row.Cells["Converted"].Value = "No";
                            WriteError(ex.ToString());
                            break;
                        }
                    }

                    // does the file exist? If not, something went wrong so set it as not 
                    if (!System.IO.File.Exists(fullpath))
                    {
                        row.Cells["Tagged"].Value = "0";
                        row.Cells["Converted"].Value = "No";
                        break;
                    }

                    // check if file is in use
                    bool fileInUse = true;
                    while (fileInUse)
                    {
                        try
                        {
                            // Attempt to open the file with FileShare.None to check if it's in use
                            using (var fileStream = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.None))
                            {
                                // if the file can be opened, then let's write the mp3 tags
                                fileInUse = false;
                            }
                        }
                        catch (IOException ex)
                        {
                            // File is still in use, wait for a short duration before trying again
                            row.Cells["Converted"].Value = "No";
                            break;
                        }
                    }

                    // create the tag file and write the tags
                    try
                    {
                        var tagfile = TagLib.File.Create(fullpath);
                        tagfile.Tag.AlbumArtists = artist;
                        tagfile.Tag.Artists = artist;
                        tagfile.Tag.Album = album;
                        tagfile.Tag.Title = song;
                        tagfile.Tag.Comment = link;
                        tagfile.Tag.Track = Convert.ToUInt16(songnum);
                        tagfile.Save();
                    }
                    catch (Exception ex)
                    {
                        row.Cells["Converted"].Value = "No";
                        break;
                    }

                    // store the tagged status
                    row.Cells["Tagged"].Value = "1";
                    row.Cells["Converted"].Value = "Yes";
                }
                else if (dlpercent == 100 && row.Cells["Tagged"].Value.ToString() == "1") // if it's already tagged, then ensure the full conversion is done
                {
                    row.Cells["Converted"].Value = "Yes";
                }
            }
        }

        private void SaveDataGridViewToCSV()
        {
            if (isSaving == true) return; //no sense saving if it's already trying
            string filePath = Application.StartupPath + "\\grid.dat";
            isSaving = true;

            try
            {
                // Create the CSV file and write the header
                using (var streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    // Write header
                    for (int i = 0; i < dgv_downloads.Columns.Count; i++)
                    {
                        streamWriter.Write(dgv_downloads.Columns[i].HeaderText);
                        if (i < dgv_downloads.Columns.Count - 1)
                            streamWriter.Write("|");
                    }
                    streamWriter.WriteLine();

                    // Write data
                    for (int i = 0; i < dgv_downloads.Rows.Count; i++)
                    {
                        for (int j = 0; j < dgv_downloads.Columns.Count; j++)
                        {
                            streamWriter.Write(dgv_downloads.Rows[i].Cells[j].Value);
                            if (j < dgv_downloads.Columns.Count - 1)
                                streamWriter.Write("|");
                        }
                        streamWriter.WriteLine();
                    }
                }

                Console.WriteLine($"Data saved to {filePath}");
            }
            catch (Exception ex)
            {
                WriteError(ex.ToString());
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
            finally
            {
                isSaving = false;
            }
        }

        private void LoadCSVIntoDataGridView()
        {
            string filePath = Application.StartupPath + "\\grid.dat";
            dgv_downloads.Rows.Clear();

            if (!System.IO.File.Exists(filePath)) return;

            try
            {
                // Read all lines from the CSV file
                string[] lines = System.IO.File.ReadAllLines(filePath, Encoding.UTF8);

                // Add rows to the DataGridView
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split('|');

                    // Create a new row
                    int rowIndex = dgv_downloads.Rows.Add();

                    // Set cell values for the row
                    for (int j = 0; j < values.Length; j++)
                    {
                        dgv_downloads.Rows[rowIndex].Cells[j].Value = values[j];
                    }

                    // if the DLPercent is not 100 or 0, then restart the download
                    //Int16 dlpercent = Convert.ToInt16(dgv_downloads.Rows[rowIndex].Cells["DL"].Value.ToString());
                    //if (dlpercent > 0 && dlpercent < 100) dgv_downloads.Rows[rowIndex].Cells["DL"].Value = "0";
                }

                dgv_downloads.Refresh();
                Console.WriteLine($"Data loaded from {filePath}");
            }
            catch (Exception ex)
            {
                WriteError(ex.ToString());
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private void timer_save_Tick(object sender, EventArgs e)
        {
            SaveDataGridViewToCSV();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                // Set the initial folder if needed
                // folderBrowserDialog.SelectedPath = "C:\\";

                // Set the title of the dialog
                folderBrowserDialog.Description = "Select a download folder";

                // Show the dialog and get the result
                DialogResult result = folderBrowserDialog.ShowDialog();

                // Check if the user clicked OK
                if (result == DialogResult.OK)
                {
                    // Get the selected folder path
                    string selectedFolder = folderBrowserDialog.SelectedPath;
                    txt_Dir.Text = selectedFolder;
                    Properties.Settings.Default.Directory = selectedFolder;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    // Handle the case where the user canceled the dialog
                    Console.WriteLine("Folder selection canceled by the user.");
                }
            }

        }

        private void txt_URL_TextChanged(object sender, EventArgs e)
        {
        }

        private async void timer_convert_Tick(object sender, EventArgs e)
        {
            //if (!Directory.Exists(txt_Dir.Text)) return;

            //// Get subdirectories
            //string[] subdirectories = Directory.GetDirectories(txt_Dir.Text);

            //// Get all files with a ".webm" extension in subdirectories
            //string[] nonMp3Files = Directory.EnumerateFiles(txt_Dir.Text, "*", SearchOption.AllDirectories)
            //    .Where(file => !file.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            //    .ToArray();

            //Console.WriteLine("\nNon-mp3 files files:" + nonMp3Files.Length);

            //// converts non-mp3 to mp3
            //foreach (string nonMp3File in nonMp3Files)
            //{
            //    foreach (DataGridViewRow row in dgv_downloads.Rows)
            //    {
            //        if (row.Cells[0].Value == null || row.Cells[0].Value.ToString().Length == 0 || row.Cells["DL"].Value.ToString() != "100") continue;
            //        // review levenshtein distance for mp3 file and song name so we only convert the mp3
            //        string foundsong = nonMp3File.Substring(nonMp3File.LastIndexOf('\\') + 2);
            //        foundsong = foundsong.Substring(foundsong.IndexOf('-') + 2, foundsong.LastIndexOf('.') - 4);
            //        string cellsong = row.Cells["Song"].Value.ToString();
            //        int lev = Levenshtein.Distance(foundsong, cellsong);

            //        // if threshold is met, then convert non-mp3 to mp3
            //        if (lev > 70)
            //        {
            //            await ConvertFile(nonMp3File, nonMp3File.Substring(nonMp3File.LastIndexOf('.')));
            //            Console.WriteLine(nonMp3File);
            //        }
            //    }
            //}
        }

        private string GetValidFilename(string inputFilename)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            // Remove invalid characters entirely
            string validFilename = new string(inputFilename.Where(c => !invalidChars.Contains(c)).ToArray());
            return validFilename;
        }

        private void btn_Pause_Click(object sender, EventArgs e)
        {
            PauseSystem();
        }

        private void PauseSystem()
        {
            isPaused = true;
            timer1.Enabled = false;
            //timer_convert.Enabled = false;
            timer_tag.Enabled = false;
            btn_Pause.Visible = false;
            btn_Resume.Visible = true;
        }

        private void btn_Resume_Click(object sender, EventArgs e)
        {
            isPaused = false;
            timer1.Enabled = true;
            //timer_convert.Enabled = true;
            timer_tag.Enabled = true;
            btn_Pause.Visible = true;
            btn_Resume.Visible = false;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isSaving != false)
            {
                e.Cancel = true;
                isPaused = true;
                timer1.Enabled = false;
                btn_Pause.Visible = false;
                btn_Resume.Visible = true;

                MessageBox.Show("Your progress is saving. I will pause progress and attempt to close again in [5] seconds.", "Close Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Use Task.Run to avoid marking the FormClosing event handler as async
                await Task.Run(async () =>
                {
                    // Introduce a CancellationToken for cleanup
                    using (var cancellationTokenSource = new CancellationTokenSource())
                    {
                        // Allow cancellation after 5 seconds
                        await Task.Delay(5000, cancellationTokenSource.Token);

                        // Ensure the cancellation is not already requested before closing
                        if (!cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            // Close the form
                            BeginInvoke(new Action(() => Close()));
                        }
                    }
                });
            }
        }

        private void dgv_downloads_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btn_secret_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void dgv_downloads_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void menu_cleardata_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure want to delete your data?", "YouTube Album Downloader", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                dgv_downloads.Rows.Clear();
                SaveDataGridViewToCSV();
            }
        }

        private void btn_secret_Click(object sender, EventArgs e)
        {

        }

        private void openDataFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Application.StartupPath);
        }

        private void RestartBadItems_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgv_downloads.Rows)
            {
                if (row.Cells[0].Value == null || row.Cells[0].Value.ToString().Length == 0) continue;

                if (row.Cells["Converted"].Value.ToString() != "Yes" && row.Cells["DL"].Value.ToString() != "0")
                {
                    row.Cells["DL"].Value = "0";
                    row.Cells["Tagged"].Value = 0;
                    row.Cells["Converted"].Value = "No";
                }

                if (row.Cells["Converted"].Value.ToString() != "Yes")
                {
                    row.Cells["Converted"].Value = "No";
                }
            }
        }

        private void timer_count_Tick(object sender, EventArgs e)
        {
            Int32 NotStarted = 0;
            Int32 Downloaded = 0;
            Int32 Failed = 0;
            Int32 Completed = 0;

            foreach (DataGridViewRow row in dgv_downloads.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString().Length > 1)
                {
                    try
                    {
                        if (row.Cells["DL"].Value.ToString() == "0") NotStarted++;
                        if (row.Cells["DL"].Value.ToString() == "100") Downloaded++;
                        if (row.Cells["DL"].Value.ToString() == "100" && row.Cells["Converted"].Value.ToString() == "No") Failed++;
                        if (row.Cells["DL"].Value.ToString() == "100" && row.Cells["Tagged"].Value.ToString() == "1") Completed++;
                    }
                    catch (Exception ex)
                    {
                        WriteError(ex.ToString());
                        break;
                    }

                    continue;
                }
            }

            lbl_status.Text = "Not Started: " + NotStarted.ToString() + "  ¤  Downloaded: " + Downloaded.ToString() + "  ¤  Failed: " + Failed.ToString() + "  ¤  Completed: " + Completed.ToString();
        }

        private void saveTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDataGridViewToCSV();
            MessageBox.Show("Your data has been saved!", "YouTube Album Downloader", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void WriteError(string msg)
        {
            string strFile = Application.StartupPath + "\\error.log";
            if (!System.IO.File.Exists(strFile)) System.IO.File.Create(strFile);

            using (var sr = new StreamWriter(strFile, true, Encoding.UTF8))
            {
                sr.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + msg);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            Form frm = new frmArtist(txt_URL.Text);
            frm.Show();
            btn_GetArtist.Enabled = false;

            while (frm.Visible == true)
            {
                Application.DoEvents();
            }

            // I'm storing the list of albums in settings because I'm an idiot with cross-form functionality
            string album = Properties.Settings.Default.AlbumData;
            Int16 albumcount = 0;

            // if the user quit the form or there's nothing to process...
            if (album == "END" || album.Length == 0)
            {
                btn_GetArtist.Enabled = true;
                return;
            }

            // if only one album
            if (!album.Contains(";"))
            {
                GetAlbumInfo2Async(album);
                albumcount++;
            }
            else // multiple albums
            {
                string[] albums = album.Split(';');
                foreach (string s in albums)
                {
                    // this is loading the pulled album from YouTube, which is not the same as the playlist
                    // WebView21 will find the redirect page and get the actual playlist URL
                    webView21.Source = new Uri(s);

                    while (albumURL.Length == 0)
                    {
                        // wait for the webview to load the redirect page
                        Application.DoEvents();
                    }

                    // we found it! Time to process it
                    GetAlbumInfo2Async(albumURL);
                    albumURL = "";
                    albumcount++;
                }
            }

            txt_URL.Text = "";
            MessageBox.Show("Completed processing " + albumcount.ToString() + " albums.", "Bulk Album Additions", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btn_GetArtist.Enabled = true;
        }

        private void webView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            // this is the final URL after all redirects
            albumURL = webView21.Source.ToString();
        }

        private async Task GetAlbumInfo2Async(string url)
        {
            // connect to the internets
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/119.0");

            // Explicitly handle encoding
            var response = await client.GetAsync(url);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            var html = Encoding.UTF8.GetString(bytes);

            // get the songs from the HTML
            var songs = YouTubeMusicExtractor.ExtractSongs(html);

            // add each song to the datagridview
            foreach (var s in songs)
            {
                dgv_downloads.Rows.Add(HttpUtility.HtmlDecode(s.Artist), HttpUtility.HtmlDecode(s.Album), s.Number, HttpUtility.HtmlDecode(s.Name), s.Duration, s.Url, "0", "0", "No");
            }

            txt_URL.Clear();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using (var fileBrowserDialog = new OpenFileDialog())
            {
                // Set the initial folder if needed
                // folderBrowserDialog.SelectedPath = "C:\\";

                // Set the title of the dialog
                fileBrowserDialog.Title = "Select a cookie file";
                fileBrowserDialog.Filter = "Text Files (*.txt)|*.txt";
                fileBrowserDialog.InitialDirectory = Application.StartupPath;

                // Show the dialog and get the result
                DialogResult result = fileBrowserDialog.ShowDialog();

                // Check if the user clicked OK
                if (result == DialogResult.OK)
                {
                    // Get the selected folder path
                    string selectedFile = fileBrowserDialog.SafeFileName;
                    txt_Cookies.Text = selectedFile;
                    Properties.Settings.Default.CookieFile = selectedFile;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    // Handle the case where the user canceled the dialog
                    Console.WriteLine("File selection canceled by the user.");
                }
            }
        }

        private void chk_Cookies_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseCookies = chk_Cookies.Checked;
            Properties.Settings.Default.Save();
        }

        private void lbl_CookieHelp_Click(object sender, EventArgs e)
        {
            Form frm = new frmCookies();
            frm.Show();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Enter the full URL copied from your favorite browser of either the album or artist page you wish to download. Then, use the related button to pull all songs from an album or select multiple albums from the artist.", "YouTube Album Downloader", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void correctArtistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv_downloads.SelectedRows.Count == 0) return;

            int rowIndex = dgv_downloads.CurrentCell?.RowIndex ?? -1;

            // get the selected band name
            string bandname = dgv_downloads.Rows[rowIndex].Cells[0].Value.ToString();

            // correct the band name
            string userInput = Interaction.InputBox("If the band name is incorrect, this is your chance to update it:", "YTAD", bandname);

            // if null then ignore
            if (userInput != null && userInput.Length > 1)
            {
                // update band name for each row with the wrong band name
                foreach (DataGridViewRow row in dgv_downloads.Rows)
                {
                    // no null cells!
                    if (row.Cells[0].Value != null && row.Cells[0].Value.ToString().Length > 1)
                    {
                        // if the band name is the same as the one that needs correcting then correct it
                        if (row.Cells[0].Value.ToString() == bandname)
                        {
                            row.Cells[0].Value = userInput;
                        }
                    }
                }

                // refresh table
                dgv_downloads.Refresh();

                // save
                SaveDataGridViewToCSV();
            }
        }

        private void dgv_downloads_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = dgv_downloads.CurrentCell?.RowIndex ?? -1;
            dgv_downloads.Rows[rowIndex].Selected = true;
        }

        private void correctAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv_downloads.SelectedRows.Count == 0) return;

            int rowIndex = dgv_downloads.CurrentCell?.RowIndex ?? -1;

            // get the selected band name
            string albumname = dgv_downloads.Rows[rowIndex].Cells["Album"].Value.ToString();

            // correct the band name
            string userInput = Interaction.InputBox("If the album name is incorrect, this is your chance to update it:", "YTAD", albumname);

            // if null then ignore
            if (userInput != null && userInput.Length > 1)
            {
                // update band name for each row with the wrong band name
                foreach (DataGridViewRow row in dgv_downloads.Rows)
                {
                    // no null cells!
                    if (row.Cells["Album"].Value != null && row.Cells["Album"].Value.ToString().Length > 1)
                    {
                        // if the album name is the same as the one that needs correcting then correct it
                        if (row.Cells["Album"].Value.ToString() == albumname)
                        {
                            row.Cells["Album"].Value = userInput;
                        }
                    }
                }

                // refresh table
                dgv_downloads.Refresh();

                // save
                SaveDataGridViewToCSV();
            }
        }

        private void correctSongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv_downloads.SelectedRows.Count == 0) return;

            int rowIndex = dgv_downloads.CurrentCell?.RowIndex ?? -1;

            // get the selected band name
            string songname = dgv_downloads.Rows[rowIndex].Cells["Song"].Value.ToString();

            // correct the band name
            string userInput = Interaction.InputBox("If the song name is incorrect, this is your chance to update it:", "YTAD", songname);

            // if null then ignore
            if (userInput != null && userInput.Length > 1)
            {
                // update song name
                dgv_downloads.Rows[rowIndex].Cells["Song"].Value = userInput;

                // refresh table
                dgv_downloads.Refresh();

                // save
                SaveDataGridViewToCSV();
            }
        }
    }
}
