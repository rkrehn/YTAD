namespace YTPD
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            label1 = new Label();
            txt_URL = new TextBox();
            btn_GetAlbum = new Button();
            dgv_downloads = new DataGridView();
            Artist = new DataGridViewTextBoxColumn();
            Album = new DataGridViewTextBoxColumn();
            SongNum = new DataGridViewTextBoxColumn();
            Song = new DataGridViewTextBoxColumn();
            Duration = new DataGridViewTextBoxColumn();
            Link = new DataGridViewTextBoxColumn();
            DL = new DataGridViewTextBoxColumn();
            Tagged = new DataGridViewTextBoxColumn();
            Converted = new DataGridViewTextBoxColumn();
            label2 = new Label();
            txt_Dir = new TextBox();
            button2 = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            timer_tag = new System.Windows.Forms.Timer(components);
            timer_convert = new System.Windows.Forms.Timer(components);
            btn_Pause = new Button();
            btn_Resume = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            menu_cleardata = new ToolStripMenuItem();
            openDataFileToolStripMenuItem = new ToolStripMenuItem();
            saveTableToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            correctArtistToolStripMenuItem = new ToolStripMenuItem();
            correctAlbumToolStripMenuItem = new ToolStripMenuItem();
            correctSongToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            RestartBadItems = new ToolStripMenuItem();
            lbl_status = new Label();
            timer_count = new System.Windows.Forms.Timer(components);
            btn_GetArtist = new Button();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            txt_Cookies = new TextBox();
            label3 = new Label();
            button1 = new Button();
            chk_Cookies = new CheckBox();
            lbl_CookieHelp = new Label();
            lbl_YTURLHelp = new Label();
            removeSongToolMenuItem1 = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)dgv_downloads).BeginInit();
            contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 15);
            label1.Name = "label1";
            label1.Size = new Size(60, 16);
            label1.TabIndex = 0;
            label1.Text = "YT URL:";
            // 
            // txt_URL
            // 
            txt_URL.Location = new Point(81, 11);
            txt_URL.Margin = new Padding(3, 2, 3, 2);
            txt_URL.Name = "txt_URL";
            txt_URL.Size = new Size(524, 23);
            txt_URL.TabIndex = 1;
            txt_URL.TextChanged += txt_URL_TextChanged;
            // 
            // btn_GetAlbum
            // 
            btn_GetAlbum.Location = new Point(611, 9);
            btn_GetAlbum.Margin = new Padding(3, 2, 3, 2);
            btn_GetAlbum.Name = "btn_GetAlbum";
            btn_GetAlbum.Size = new Size(76, 25);
            btn_GetAlbum.TabIndex = 2;
            btn_GetAlbum.Text = "+ Album";
            btn_GetAlbum.UseVisualStyleBackColor = true;
            btn_GetAlbum.Click += button1_Click;
            // 
            // dgv_downloads
            // 
            dgv_downloads.BackgroundColor = Color.Azure;
            dgv_downloads.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = Color.Azure;
            dataGridViewCellStyle4.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle4.ForeColor = Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            dgv_downloads.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dgv_downloads.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_downloads.Columns.AddRange(new DataGridViewColumn[] { Artist, Album, SongNum, Song, Duration, Link, DL, Tagged, Converted });
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.Azure;
            dataGridViewCellStyle6.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle6.ForeColor = Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dgv_downloads.DefaultCellStyle = dataGridViewCellStyle6;
            dgv_downloads.Location = new Point(12, 98);
            dgv_downloads.Margin = new Padding(3, 2, 3, 2);
            dgv_downloads.Name = "dgv_downloads";
            dgv_downloads.Size = new Size(775, 322);
            dgv_downloads.TabIndex = 3;
            dgv_downloads.CellClick += dgv_downloads_CellClick;
            dgv_downloads.CellDoubleClick += dgv_downloads_CellDoubleClick;
            dgv_downloads.MouseDown += dgv_downloads_MouseDown;
            // 
            // Artist
            // 
            dataGridViewCellStyle5.BackColor = Color.Azure;
            dataGridViewCellStyle5.ForeColor = Color.Black;
            Artist.DefaultCellStyle = dataGridViewCellStyle5;
            Artist.HeaderText = "Artist";
            Artist.Name = "Artist";
            Artist.Width = 150;
            // 
            // Album
            // 
            Album.HeaderText = "Album";
            Album.Name = "Album";
            Album.Width = 150;
            // 
            // SongNum
            // 
            SongNum.HeaderText = "#";
            SongNum.Name = "SongNum";
            SongNum.Width = 25;
            // 
            // Song
            // 
            Song.HeaderText = "Song";
            Song.Name = "Song";
            Song.Width = 200;
            // 
            // Duration
            // 
            Duration.HeaderText = "Duration";
            Duration.Name = "Duration";
            Duration.Width = 75;
            // 
            // Link
            // 
            Link.HeaderText = "Link";
            Link.Name = "Link";
            Link.Visible = false;
            // 
            // DL
            // 
            DL.HeaderText = "DL %";
            DL.Name = "DL";
            DL.Width = 40;
            // 
            // Tagged
            // 
            Tagged.HeaderText = "Tagged";
            Tagged.Name = "Tagged";
            Tagged.Visible = false;
            // 
            // Converted
            // 
            Converted.HeaderText = "Converted";
            Converted.Name = "Converted";
            Converted.Width = 90;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 45);
            label2.Name = "label2";
            label2.Size = new Size(121, 16);
            label2.TabIndex = 4;
            label2.Text = "Download Folder:";
            // 
            // txt_Dir
            // 
            txt_Dir.Enabled = false;
            txt_Dir.Location = new Point(136, 42);
            txt_Dir.Margin = new Padding(3, 2, 3, 2);
            txt_Dir.Name = "txt_Dir";
            txt_Dir.Size = new Size(583, 23);
            txt_Dir.TabIndex = 5;
            txt_Dir.Text = "C:\\Users\\satsu\\Downloads";
            // 
            // button2
            // 
            button2.Location = new Point(725, 41);
            button2.Margin = new Padding(3, 2, 3, 2);
            button2.Name = "button2";
            button2.Size = new Size(65, 25);
            button2.TabIndex = 6;
            button2.Text = "Browse";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // timer_tag
            // 
            timer_tag.Interval = 10000;
            timer_tag.Tick += timer_tag_Tick;
            // 
            // timer_convert
            // 
            timer_convert.Interval = 30000;
            timer_convert.Tick += timer_convert_Tick;
            // 
            // btn_Pause
            // 
            btn_Pause.Location = new Point(12, 429);
            btn_Pause.Name = "btn_Pause";
            btn_Pause.Size = new Size(75, 23);
            btn_Pause.TabIndex = 7;
            btn_Pause.Text = "Pause";
            btn_Pause.UseVisualStyleBackColor = true;
            btn_Pause.Visible = false;
            btn_Pause.Click += btn_Pause_Click;
            // 
            // btn_Resume
            // 
            btn_Resume.Location = new Point(712, 429);
            btn_Resume.Name = "btn_Resume";
            btn_Resume.Size = new Size(75, 23);
            btn_Resume.TabIndex = 8;
            btn_Resume.Text = "Resume";
            btn_Resume.UseVisualStyleBackColor = true;
            btn_Resume.Click += btn_Resume_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { menu_cleardata, openDataFileToolStripMenuItem, saveTableToolStripMenuItem, toolStripSeparator1, correctArtistToolStripMenuItem, correctAlbumToolStripMenuItem, correctSongToolStripMenuItem, removeSongToolMenuItem1, toolStripSeparator2, RestartBadItems });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(213, 214);
            // 
            // menu_cleardata
            // 
            menu_cleardata.Name = "menu_cleardata";
            menu_cleardata.Size = new Size(212, 22);
            menu_cleardata.Text = "Clear Data";
            menu_cleardata.Click += menu_cleardata_Click;
            // 
            // openDataFileToolStripMenuItem
            // 
            openDataFileToolStripMenuItem.Name = "openDataFileToolStripMenuItem";
            openDataFileToolStripMenuItem.Size = new Size(212, 22);
            openDataFileToolStripMenuItem.Text = "Open Data Folder";
            openDataFileToolStripMenuItem.Click += openDataFileToolStripMenuItem_Click;
            // 
            // saveTableToolStripMenuItem
            // 
            saveTableToolStripMenuItem.Name = "saveTableToolStripMenuItem";
            saveTableToolStripMenuItem.Size = new Size(212, 22);
            saveTableToolStripMenuItem.Text = "Save Table";
            saveTableToolStripMenuItem.Click += saveTableToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(209, 6);
            // 
            // correctArtistToolStripMenuItem
            // 
            correctArtistToolStripMenuItem.Name = "correctArtistToolStripMenuItem";
            correctArtistToolStripMenuItem.Size = new Size(212, 22);
            correctArtistToolStripMenuItem.Text = "Correct Artist";
            correctArtistToolStripMenuItem.Click += correctArtistToolStripMenuItem_Click;
            // 
            // correctAlbumToolStripMenuItem
            // 
            correctAlbumToolStripMenuItem.Name = "correctAlbumToolStripMenuItem";
            correctAlbumToolStripMenuItem.Size = new Size(212, 22);
            correctAlbumToolStripMenuItem.Text = "Correct Album";
            correctAlbumToolStripMenuItem.Click += correctAlbumToolStripMenuItem_Click;
            // 
            // correctSongToolStripMenuItem
            // 
            correctSongToolStripMenuItem.Name = "correctSongToolStripMenuItem";
            correctSongToolStripMenuItem.Size = new Size(212, 22);
            correctSongToolStripMenuItem.Text = "Correct Song";
            correctSongToolStripMenuItem.Click += correctSongToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(209, 6);
            // 
            // RestartBadItems
            // 
            RestartBadItems.Name = "RestartBadItems";
            RestartBadItems.Size = new Size(212, 22);
            RestartBadItems.Text = "Restart Broken Downloads";
            RestartBadItems.Click += RestartBadItems_Click;
            // 
            // lbl_status
            // 
            lbl_status.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbl_status.Location = new Point(96, 433);
            lbl_status.Name = "lbl_status";
            lbl_status.Size = new Size(613, 15);
            lbl_status.TabIndex = 9;
            lbl_status.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // timer_count
            // 
            timer_count.Enabled = true;
            timer_count.Interval = 10000;
            timer_count.Tick += timer_count_Tick;
            // 
            // btn_GetArtist
            // 
            btn_GetArtist.Location = new Point(693, 9);
            btn_GetArtist.Margin = new Padding(3, 2, 3, 2);
            btn_GetArtist.Name = "btn_GetArtist";
            btn_GetArtist.Size = new Size(75, 25);
            btn_GetArtist.TabIndex = 12;
            btn_GetArtist.Text = "+ Artist";
            btn_GetArtist.UseVisualStyleBackColor = true;
            btn_GetArtist.Click += button3_Click_2;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Location = new Point(778, 429);
            webView21.Name = "webView21";
            webView21.Size = new Size(19, 29);
            webView21.TabIndex = 13;
            webView21.Visible = false;
            webView21.ZoomFactor = 1D;
            webView21.NavigationCompleted += webView21_NavigationCompleted;
            // 
            // txt_Cookies
            // 
            txt_Cookies.Enabled = false;
            txt_Cookies.Location = new Point(104, 69);
            txt_Cookies.Margin = new Padding(3, 2, 3, 2);
            txt_Cookies.Name = "txt_Cookies";
            txt_Cookies.Size = new Size(481, 23);
            txt_Cookies.TabIndex = 15;
            txt_Cookies.Text = "C:\\Users\\satsu\\Downloads";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 72);
            label3.Name = "label3";
            label3.Size = new Size(83, 16);
            label3.TabIndex = 14;
            label3.Text = "Cookie File:";
            // 
            // button1
            // 
            button1.Location = new Point(591, 67);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(65, 25);
            button1.TabIndex = 16;
            button1.Text = "Browse";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // chk_Cookies
            // 
            chk_Cookies.AutoSize = true;
            chk_Cookies.Location = new Point(662, 71);
            chk_Cookies.Name = "chk_Cookies";
            chk_Cookies.Size = new Size(105, 20);
            chk_Cookies.TabIndex = 17;
            chk_Cookies.Text = "Use Cookies";
            chk_Cookies.UseVisualStyleBackColor = true;
            chk_Cookies.CheckedChanged += chk_Cookies_CheckedChanged;
            // 
            // lbl_CookieHelp
            // 
            lbl_CookieHelp.AutoSize = true;
            lbl_CookieHelp.BackColor = Color.DodgerBlue;
            lbl_CookieHelp.FlatStyle = FlatStyle.Popup;
            lbl_CookieHelp.ForeColor = Color.White;
            lbl_CookieHelp.Location = new Point(773, 72);
            lbl_CookieHelp.Name = "lbl_CookieHelp";
            lbl_CookieHelp.Size = new Size(14, 16);
            lbl_CookieHelp.TabIndex = 18;
            lbl_CookieHelp.Text = "?";
            lbl_CookieHelp.Click += lbl_CookieHelp_Click;
            // 
            // lbl_YTURLHelp
            // 
            lbl_YTURLHelp.AutoSize = true;
            lbl_YTURLHelp.BackColor = Color.DodgerBlue;
            lbl_YTURLHelp.FlatStyle = FlatStyle.Popup;
            lbl_YTURLHelp.ForeColor = Color.White;
            lbl_YTURLHelp.Location = new Point(774, 14);
            lbl_YTURLHelp.Name = "lbl_YTURLHelp";
            lbl_YTURLHelp.Size = new Size(14, 16);
            lbl_YTURLHelp.TabIndex = 19;
            lbl_YTURLHelp.Text = "?";
            lbl_YTURLHelp.Click += label4_Click;
            // 
            // removeSongToolMenuItem1
            // 
            removeSongToolMenuItem1.Name = "removeSongToolMenuItem1";
            removeSongToolMenuItem1.Size = new Size(212, 22);
            removeSongToolMenuItem1.Text = "Remove Song";
            removeSongToolMenuItem1.Click += removeSongToolMenuItem1_Click;
            // 
            // Form1
            // 
            AcceptButton = btn_GetAlbum;
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Azure;
            ClientSize = new Size(802, 458);
            Controls.Add(lbl_YTURLHelp);
            Controls.Add(lbl_CookieHelp);
            Controls.Add(chk_Cookies);
            Controls.Add(button1);
            Controls.Add(txt_Cookies);
            Controls.Add(label3);
            Controls.Add(webView21);
            Controls.Add(btn_GetArtist);
            Controls.Add(lbl_status);
            Controls.Add(btn_Resume);
            Controls.Add(btn_Pause);
            Controls.Add(button2);
            Controls.Add(txt_Dir);
            Controls.Add(label2);
            Controls.Add(dgv_downloads);
            Controls.Add(btn_GetAlbum);
            Controls.Add(txt_URL);
            Controls.Add(label1);
            Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "Form1";
            Text = "YouTube Album Downloader v2.5";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgv_downloads).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txt_URL;
        private Button btn_GetAlbum;
        private DataGridView dgv_downloads;
        private Label label2;
        private TextBox txt_Dir;
        private Button button2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer_tag;
        private System.Windows.Forms.Timer timer_convert;
        private Button btn_Pause;
        private Button btn_Resume;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem menu_cleardata;
        private ToolStripMenuItem openDataFileToolStripMenuItem;
        private ToolStripMenuItem RestartBadItems;
        private Label lbl_status;
        private System.Windows.Forms.Timer timer_count;
        private ToolStripMenuItem saveTableToolStripMenuItem;
        public Button btn_GetArtist;
        private Button button3;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private TextBox txt_Cookies;
        private Label label3;
        private Button button1;
        private CheckBox chk_Cookies;
        private Label lbl_CookieHelp;
        private Label lbl_YTURLHelp;
        private ToolStripMenuItem correctArtistToolStripMenuItem;
        private ToolStripMenuItem correctAlbumToolStripMenuItem;
        private ToolStripMenuItem correctSongToolStripMenuItem;
        private DataGridViewTextBoxColumn Artist;
        private DataGridViewTextBoxColumn Album;
        private DataGridViewTextBoxColumn SongNum;
        private DataGridViewTextBoxColumn Song;
        private DataGridViewTextBoxColumn Duration;
        private DataGridViewTextBoxColumn Link;
        private DataGridViewTextBoxColumn DL;
        private DataGridViewTextBoxColumn Tagged;
        private DataGridViewTextBoxColumn Converted;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem removeSongToolMenuItem1;
    }
}
