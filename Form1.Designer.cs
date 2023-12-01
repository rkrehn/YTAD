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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            label1 = new Label();
            txt_URL = new TextBox();
            button1 = new Button();
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
            timer_save = new System.Windows.Forms.Timer(components);
            timer_convert = new System.Windows.Forms.Timer(components);
            btn_Pause = new Button();
            btn_Resume = new Button();
            ((System.ComponentModel.ISupportInitialize)dgv_downloads).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 14);
            label1.Name = "label1";
            label1.Size = new Size(81, 16);
            label1.TabIndex = 0;
            label1.Text = "Album URL:";
            // 
            // txt_URL
            // 
            txt_URL.Location = new Point(100, 11);
            txt_URL.Margin = new Padding(3, 2, 3, 2);
            txt_URL.Name = "txt_URL";
            txt_URL.Size = new Size(644, 23);
            txt_URL.TabIndex = 1;
            txt_URL.TextChanged += txt_URL_TextChanged;
            // 
            // button1
            // 
            button1.Location = new Point(750, 7);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(37, 25);
            button1.TabIndex = 2;
            button1.Text = "Go";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dgv_downloads
            // 
            dgv_downloads.BackgroundColor = Color.Azure;
            dgv_downloads.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.Azure;
            dataGridViewCellStyle1.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgv_downloads.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgv_downloads.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_downloads.Columns.AddRange(new DataGridViewColumn[] { Artist, Album, SongNum, Song, Duration, Link, DL, Tagged, Converted });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.Azure;
            dataGridViewCellStyle3.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgv_downloads.DefaultCellStyle = dataGridViewCellStyle3;
            dgv_downloads.Location = new Point(12, 69);
            dgv_downloads.Margin = new Padding(3, 2, 3, 2);
            dgv_downloads.Name = "dgv_downloads";
            dgv_downloads.Size = new Size(775, 322);
            dgv_downloads.TabIndex = 3;
            dgv_downloads.CellContentClick += dgv_downloads_CellContentClick;
            // 
            // Artist
            // 
            dataGridViewCellStyle2.BackColor = Color.Azure;
            dataGridViewCellStyle2.ForeColor = Color.Black;
            Artist.DefaultCellStyle = dataGridViewCellStyle2;
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
            Converted.Width = 80;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 44);
            label2.Name = "label2";
            label2.Size = new Size(121, 16);
            label2.TabIndex = 4;
            label2.Text = "Download Folder:";
            // 
            // txt_Dir
            // 
            txt_Dir.Enabled = false;
            txt_Dir.Location = new Point(133, 41);
            txt_Dir.Margin = new Padding(3, 2, 3, 2);
            txt_Dir.Name = "txt_Dir";
            txt_Dir.Size = new Size(583, 23);
            txt_Dir.TabIndex = 5;
            txt_Dir.Text = "C:\\Users\\satsu\\Downloads";
            // 
            // button2
            // 
            button2.Location = new Point(722, 40);
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
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // timer_tag
            // 
            timer_tag.Enabled = true;
            timer_tag.Interval = 10000;
            timer_tag.Tick += timer_tag_Tick;
            // 
            // timer_save
            // 
            timer_save.Enabled = true;
            timer_save.Interval = 30000;
            timer_save.Tick += timer_save_Tick;
            // 
            // timer_convert
            // 
            timer_convert.Enabled = true;
            timer_convert.Interval = 5000;
            timer_convert.Tick += timer_convert_Tick;
            // 
            // btn_Pause
            // 
            btn_Pause.Location = new Point(12, 400);
            btn_Pause.Name = "btn_Pause";
            btn_Pause.Size = new Size(75, 23);
            btn_Pause.TabIndex = 7;
            btn_Pause.Text = "Pause";
            btn_Pause.UseVisualStyleBackColor = true;
            btn_Pause.Click += btn_Pause_Click;
            // 
            // btn_Resume
            // 
            btn_Resume.Location = new Point(712, 400);
            btn_Resume.Name = "btn_Resume";
            btn_Resume.Size = new Size(75, 23);
            btn_Resume.TabIndex = 8;
            btn_Resume.Text = "Resume";
            btn_Resume.UseVisualStyleBackColor = true;
            btn_Resume.Visible = false;
            btn_Resume.Click += btn_Resume_Click;
            // 
            // Form1
            // 
            AcceptButton = button1;
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Azure;
            ClientSize = new Size(802, 432);
            Controls.Add(btn_Resume);
            Controls.Add(btn_Pause);
            Controls.Add(button2);
            Controls.Add(txt_Dir);
            Controls.Add(label2);
            Controls.Add(dgv_downloads);
            Controls.Add(button1);
            Controls.Add(txt_URL);
            Controls.Add(label1);
            Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "Form1";
            Text = "YouTube Album Downloader";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgv_downloads).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txt_URL;
        private Button button1;
        private DataGridView dgv_downloads;
        private Label label2;
        private TextBox txt_Dir;
        private Button button2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer_tag;
        private System.Windows.Forms.Timer timer_save;
        private System.Windows.Forms.Timer timer_convert;
        private DataGridViewTextBoxColumn Artist;
        private DataGridViewTextBoxColumn Album;
        private DataGridViewTextBoxColumn SongNum;
        private DataGridViewTextBoxColumn Song;
        private DataGridViewTextBoxColumn Duration;
        private DataGridViewTextBoxColumn Link;
        private DataGridViewTextBoxColumn DL;
        private DataGridViewTextBoxColumn Tagged;
        private DataGridViewTextBoxColumn Converted;
        private Button btn_Pause;
        private Button btn_Resume;
    }
}
