namespace YTPD.Properties
{
    partial class frmArtist
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblArtist = new Label();
            dataDisco = new DataGridView();
            Select = new DataGridViewCheckBoxColumn();
            AlbumName = new DataGridViewTextBoxColumn();
            Playlist = new DataGridViewTextBoxColumn();
            btn_Process = new Button();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataDisco).BeginInit();
            SuspendLayout();
            // 
            // lblArtist
            // 
            lblArtist.AutoSize = true;
            lblArtist.Location = new Point(12, 9);
            lblArtist.Name = "lblArtist";
            lblArtist.Size = new Size(114, 15);
            lblArtist.TabIndex = 0;
            lblArtist.Text = "Loading artist data...";
            // 
            // dataDisco
            // 
            dataDisco.BackgroundColor = Color.Azure;
            dataDisco.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataDisco.Columns.AddRange(new DataGridViewColumn[] { Select, AlbumName, Playlist });
            dataDisco.Location = new Point(11, 27);
            dataDisco.Name = "dataDisco";
            dataDisco.Size = new Size(495, 229);
            dataDisco.TabIndex = 1;
            // 
            // Select
            // 
            Select.HeaderText = "Select";
            Select.Name = "Select";
            Select.Width = 45;
            // 
            // AlbumName
            // 
            AlbumName.HeaderText = "Album Name";
            AlbumName.Name = "AlbumName";
            AlbumName.Width = 410;
            // 
            // Playlist
            // 
            Playlist.HeaderText = "Playlist";
            Playlist.Name = "Playlist";
            Playlist.Visible = false;
            // 
            // btn_Process
            // 
            btn_Process.Location = new Point(444, 288);
            btn_Process.Name = "btn_Process";
            btn_Process.Size = new Size(63, 23);
            btn_Process.TabIndex = 2;
            btn_Process.Text = "Process";
            btn_Process.UseVisualStyleBackColor = true;
            btn_Process.Click += btn_Process_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 259);
            label1.Name = "label1";
            label1.Size = new Size(472, 15);
            label1.TabIndex = 3;
            label1.Text = "Select one or more albums above, then wait a few minutes while we process the albums.";
            // 
            // frmArtist
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Azure;
            ClientSize = new Size(518, 316);
            Controls.Add(label1);
            Controls.Add(btn_Process);
            Controls.Add(dataDisco);
            Controls.Add(lblArtist);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "frmArtist";
            Text = "Bulk Add Artist Albums";
            FormClosing += frmArtist_FormClosing;
            Load += frmArtist_Load;
            ((System.ComponentModel.ISupportInitialize)dataDisco).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblArtist;
        private DataGridView dataDisco;
        private Button btn_Process;
        private DataGridViewCheckBoxColumn Select;
        private DataGridViewTextBoxColumn AlbumName;
        private DataGridViewTextBoxColumn Playlist;
        private Label label1;
    }
}