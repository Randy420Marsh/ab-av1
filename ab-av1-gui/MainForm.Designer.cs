using System;
using System.Windows.Forms;

namespace ab_av1_gui
{
    public partial class MainForm : Form
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
            this.BtnStartEncoding = new System.Windows.Forms.Button();
            this.listBoxQueue = new System.Windows.Forms.ListBox();
            this.chkDeleteOriginal = new System.Windows.Forms.CheckBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.BtnAddFiles = new System.Windows.Forms.Button();
            this.BtnRemoveSelected = new System.Windows.Forms.Button();
            this.BtnStopEncoding = new System.Windows.Forms.Button();
            this.txtEncodingCommand = new System.Windows.Forms.TextBox();
            this.chkDarkMode = new System.Windows.Forms.CheckBox();
            this.lblMaxConcurrentEncodes = new System.Windows.Forms.Label();
            this.numericMaxConcurrentEncodes = new System.Windows.Forms.NumericUpDown();
            this.lblCpuCoreLimit = new System.Windows.Forms.Label();
            this.numericCpuCoreLimit = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxConcurrentEncodes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericCpuCoreLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnStartEncoding
            // 
            this.BtnStartEncoding.Location = new System.Drawing.Point(12, 112);
            this.BtnStartEncoding.Name = "BtnStartEncoding";
            this.BtnStartEncoding.Size = new System.Drawing.Size(100, 30);
            this.BtnStartEncoding.TabIndex = 0;
            this.BtnStartEncoding.Text = "Start Encoding";
            this.BtnStartEncoding.UseVisualStyleBackColor = true;
            this.BtnStartEncoding.Click += new System.EventHandler(this.BtnStartEncoding_Click);
            // 
            // listBoxQueue
            // 
            this.listBoxQueue.FormattingEnabled = true;
            this.listBoxQueue.Location = new System.Drawing.Point(12, 177);
            this.listBoxQueue.Name = "listBoxQueue";
            this.listBoxQueue.ScrollAlwaysVisible = true;
            this.listBoxQueue.Size = new System.Drawing.Size(1133, 264);
            this.listBoxQueue.TabIndex = 1;
            // 
            // chkDeleteOriginal
            // 
            this.chkDeleteOriginal.AutoSize = true;
            this.chkDeleteOriginal.Location = new System.Drawing.Point(953, 120);
            this.chkDeleteOriginal.Name = "chkDeleteOriginal";
            this.chkDeleteOriginal.Size = new System.Drawing.Size(192, 17);
            this.chkDeleteOriginal.TabIndex = 2;
            this.chkDeleteOriginal.Text = "Delete Original Files After Encoding";
            this.chkDeleteOriginal.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(12, 458);
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(1133, 250);
            this.txtLog.TabIndex = 3;
            this.txtLog.Text = "";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 148);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1133, 23);
            this.progressBar1.TabIndex = 4;
            // 
            // BtnAddFiles
            // 
            this.BtnAddFiles.Location = new System.Drawing.Point(118, 112);
            this.BtnAddFiles.Name = "BtnAddFiles";
            this.BtnAddFiles.Size = new System.Drawing.Size(100, 30);
            this.BtnAddFiles.TabIndex = 6;
            this.BtnAddFiles.Text = "Add Files";
            this.BtnAddFiles.UseVisualStyleBackColor = true;
            this.BtnAddFiles.Click += new System.EventHandler(this.BtnAddFiles_Click);
            // 
            // BtnRemoveSelected
            // 
            this.BtnRemoveSelected.Location = new System.Drawing.Point(224, 112);
            this.BtnRemoveSelected.Name = "BtnRemoveSelected";
            this.BtnRemoveSelected.Size = new System.Drawing.Size(100, 30);
            this.BtnRemoveSelected.TabIndex = 7;
            this.BtnRemoveSelected.Text = "Remove Selected";
            this.BtnRemoveSelected.UseVisualStyleBackColor = true;
            this.BtnRemoveSelected.Click += new System.EventHandler(this.BtnRemoveSelected_Click);
            // 
            // BtnStopEncoding
            // 
            this.BtnStopEncoding.Location = new System.Drawing.Point(330, 112);
            this.BtnStopEncoding.Name = "BtnStopEncoding";
            this.BtnStopEncoding.Size = new System.Drawing.Size(100, 30);
            this.BtnStopEncoding.TabIndex = 8;
            this.BtnStopEncoding.Text = "Stop Encoding";
            this.BtnStopEncoding.UseVisualStyleBackColor = true;
            this.BtnStopEncoding.Click += new System.EventHandler(this.BtnStopEncoding_Click);
            // 
            // txtEncodingCommand
            // 
            this.txtEncodingCommand.Location = new System.Drawing.Point(12, 12);
            this.txtEncodingCommand.Multiline = true;
            this.txtEncodingCommand.Name = "txtEncodingCommand";
            this.txtEncodingCommand.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEncodingCommand.Size = new System.Drawing.Size(1133, 60);
            this.txtEncodingCommand.TabIndex = 9;
            this.txtEncodingCommand.Text = "auto-encode --encoder h264_nvenc --input \"{filePath}\" --preset slow --pix-format " +
    "yuv420p --keyint 120 -o \"{outputFile}\"";
            this.txtEncodingCommand.TextChanged += new System.EventHandler(this.txtEncodingCommand_TextChanged);
            // 
            // chkDarkMode
            // 
            this.chkDarkMode.AutoSize = true;
            this.chkDarkMode.Location = new System.Drawing.Point(12, 89);
            this.chkDarkMode.Name = "chkDarkMode";
            this.chkDarkMode.Size = new System.Drawing.Size(75, 17);
            this.chkDarkMode.TabIndex = 10;
            this.chkDarkMode.Text = "Darkmode";
            this.chkDarkMode.UseVisualStyleBackColor = true;
            this.chkDarkMode.CheckedChanged += new System.EventHandler(this.chkDarkMode_CheckedChanged);
            // 
            // lblMaxConcurrentEncodes
            // 
            this.lblMaxConcurrentEncodes.AutoSize = true;
            this.lblMaxConcurrentEncodes.Location = new System.Drawing.Point(440, 90);
            this.lblMaxConcurrentEncodes.Name = "lblMaxConcurrentEncodes";
            this.lblMaxConcurrentEncodes.Size = new System.Drawing.Size(130, 13);
            this.lblMaxConcurrentEncodes.TabIndex = 11;
            this.lblMaxConcurrentEncodes.Text = "Max Concurrent Encodes:";
            // 
            // numericMaxConcurrentEncodes
            // 
            this.numericMaxConcurrentEncodes.Location = new System.Drawing.Point(581, 88);
            this.numericMaxConcurrentEncodes.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericMaxConcurrentEncodes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericMaxConcurrentEncodes.Name = "numericMaxConcurrentEncodes";
            this.numericMaxConcurrentEncodes.Size = new System.Drawing.Size(50, 20);
            this.numericMaxConcurrentEncodes.TabIndex = 12;
            this.numericMaxConcurrentEncodes.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericMaxConcurrentEncodes.ValueChanged += new System.EventHandler(this.numericMaxConcurrentEncodes_ValueChanged);
            // 
            // lblCpuCoreLimit
            // 
            this.lblCpuCoreLimit.AutoSize = true;
            this.lblCpuCoreLimit.Location = new System.Drawing.Point(650, 90);
            this.lblCpuCoreLimit.Name = "lblCpuCoreLimit";
            this.lblCpuCoreLimit.Size = new System.Drawing.Size(146, 13);
            this.lblCpuCoreLimit.TabIndex = 13;
            this.lblCpuCoreLimit.Text = "Limit Encoding to CPU Cores:";
            // 
            // numericCpuCoreLimit
            // 
            this.numericCpuCoreLimit.Location = new System.Drawing.Point(803, 88);
            this.numericCpuCoreLimit.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numericCpuCoreLimit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericCpuCoreLimit.Name = "numericCpuCoreLimit";
            this.numericCpuCoreLimit.Size = new System.Drawing.Size(50, 20);
            this.numericCpuCoreLimit.TabIndex = 14;
            this.numericCpuCoreLimit.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericCpuCoreLimit.ValueChanged += new System.EventHandler(this.numericCpuCoreLimit_ValueChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1157, 720);
            this.Controls.Add(this.numericCpuCoreLimit);
            this.Controls.Add(this.lblCpuCoreLimit);
            this.Controls.Add(this.numericMaxConcurrentEncodes);
            this.Controls.Add(this.lblMaxConcurrentEncodes);
            this.Controls.Add(this.chkDarkMode);
            this.Controls.Add(this.txtEncodingCommand);
            this.Controls.Add(this.BtnStopEncoding);
            this.Controls.Add(this.BtnRemoveSelected);
            this.Controls.Add(this.BtnAddFiles);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.chkDeleteOriginal);
            this.Controls.Add(this.listBoxQueue);
            this.Controls.Add(this.BtnStartEncoding);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxConcurrentEncodes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericCpuCoreLimit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnStartEncoding;
        private System.Windows.Forms.ListBox listBoxQueue;
        private System.Windows.Forms.CheckBox chkDeleteOriginal;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button BtnAddFiles;
        private System.Windows.Forms.Button BtnRemoveSelected;
        private System.Windows.Forms.Button BtnStopEncoding;
        private System.Windows.Forms.TextBox txtEncodingCommand;
        private CheckBox chkDarkMode;
        private System.Windows.Forms.Label lblMaxConcurrentEncodes;
        private System.Windows.Forms.NumericUpDown numericMaxConcurrentEncodes;
        private System.Windows.Forms.Label lblCpuCoreLimit; // New declaration
        private System.Windows.Forms.NumericUpDown numericCpuCoreLimit; // New declaration
    }
}