namespace TinyYolo
{
    partial class TinyYoloDemo
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
            this.SrcPictureBox = new System.Windows.Forms.PictureBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ResultList = new System.Windows.Forms.ListBox();
            this.DetectButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.SrcPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // SrcPictureBox
            // 
            this.SrcPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SrcPictureBox.Location = new System.Drawing.Point(14, 41);
            this.SrcPictureBox.Name = "SrcPictureBox";
            this.SrcPictureBox.Size = new System.Drawing.Size(478, 460);
            this.SrcPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.SrcPictureBox.TabIndex = 3;
            this.SrcPictureBox.TabStop = false;
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(101, 12);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.BrowseButton.TabIndex = 4;
            this.BrowseButton.Text = "Browse..";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Select an Image";
            // 
            // ResultList
            // 
            this.ResultList.FormattingEnabled = true;
            this.ResultList.Location = new System.Drawing.Point(498, 42);
            this.ResultList.Name = "ResultList";
            this.ResultList.Size = new System.Drawing.Size(246, 459);
            this.ResultList.TabIndex = 6;
            // 
            // DetectButton
            // 
            this.DetectButton.Enabled = false;
            this.DetectButton.Location = new System.Drawing.Point(498, 12);
            this.DetectButton.Name = "DetectButton";
            this.DetectButton.Size = new System.Drawing.Size(75, 23);
            this.DetectButton.TabIndex = 7;
            this.DetectButton.Text = "Detect";
            this.DetectButton.UseVisualStyleBackColor = true;
            this.DetectButton.Click += new System.EventHandler(this.DetectButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(182, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(171, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Analysing the Image, Please wait ..";
            this.label2.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(369, 12);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(123, 23);
            this.progressBar1.TabIndex = 9;
            this.progressBar1.Visible = false;
            // 
            // TinyYoloDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 513);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DetectButton);
            this.Controls.Add(this.ResultList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.SrcPictureBox);
            this.MaximizeBox = false;
            this.Name = "TinyYoloDemo";
            this.Text = "TinyYoloDemo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TinyYoloDemo_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.SrcPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox SrcPictureBox;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox ResultList;
        private System.Windows.Forms.Button DetectButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}

