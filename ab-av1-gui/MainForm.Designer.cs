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
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1157, 720);
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
    }
}

