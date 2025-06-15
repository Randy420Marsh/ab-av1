using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ab_av1_gui
{
    public partial class MainForm : Form
    {
        private Process encodingProcess;

        public MainForm()
        {
            InitializeComponent();
            txtEncodingCommand.Text = "auto-encode --encoder h264_nvenc --input {filePath} --preset slow --pix-format yuv420p --keyint 120 -o {outputFile}";
        }

        private void BtnAddFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Video Files|*.mp4;*.mkv;*.avi;*.webm;*.mov",
                Title = "Select Files to Encode"
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in openFileDialog.FileNames)
                    {
                        // Store the raw file path, we'll quote it when constructing the command
                        listBoxQueue.Items.Add(file);
                    }
                }
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            // You can initialize anything here if needed
        }


        private void BtnRemoveSelected_Click(object sender, EventArgs e)
        {
            while (listBoxQueue.SelectedItems.Count > 0)
            {
                listBoxQueue.Items.Remove(listBoxQueue.SelectedItem);
            }
        }

        private async void BtnStartEncoding_Click(object sender, EventArgs e)
        {
            if (listBoxQueue.Items.Count == 0)
            {
                MessageBox.Show("Please add files to the queue.", "No Files", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            progressBar1.Value = 0; // Reset progress bar
            progressBar1.Maximum = listBoxQueue.Items.Count; // Set max value

            foreach (string item in listBoxQueue.Items)
            {
                string filePath = item.ToString();
                await Task.Run(() => EncodeFile(filePath)); // Run encoding in a separate thread
            }
        }


        private void BtnStopEncoding_Click(object sender, EventArgs e)
        {
            if (encodingProcess != null && !encodingProcess.HasExited)
            {
                encodingProcess.Kill();
                encodingProcess.Dispose();
                encodingProcess = null;
                AppendLog("Encoding stopped by user.");
            }
        }


        private void EncodeFile(string filePath)
        {
            // The filePath passed here is already the raw path from listBoxQueue.Items
            // We need to ensure it's properly quoted for the command line.

            string escapedFilePath = $"\"{filePath}\""; // Correctly quote the input path
            
            // Generate the output file path. Ensure it's correctly quoted when used in the command.
            string outputDirectory = Path.GetDirectoryName(filePath);
            string outputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string outputFile = Path.Combine(outputDirectory, outputFileNameWithoutExtension + "_encoded.mkv");
            
            string escapedOutputFile = $"\"{outputFile}\""; // Correctly quote the output path

            // Replace placeholders in the encoding command with the properly quoted paths
            string commandArguments = txtEncodingCommand.Text
                .Replace("{filePath}", escapedFilePath)
                .Replace("{outputFile}", escapedOutputFile);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "ab-av1.exe",
                Arguments = commandArguments, // Pass the entire command string as arguments
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            encodingProcess = new Process { StartInfo = psi };
            encodingProcess.OutputDataReceived += (s, e) => AppendLog(e.Data);
            encodingProcess.ErrorDataReceived += (s, e) => AppendLog(e.Data);
            encodingProcess.Exited += (s, e) => OnEncodingFinished();

            encodingProcess.EnableRaisingEvents = true;
            encodingProcess.Start();
            encodingProcess.BeginOutputReadLine();
            encodingProcess.BeginErrorReadLine();
        }

        private void OnEncodingFinished()
        {
            Invoke(new Action(() =>
            {
                if (progressBar1.Value < progressBar1.Maximum)
                    progressBar1.Value += 1;
            }));
        }

        private void AppendLog(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Invoke(new Action(() => txtLog.AppendText(text + Environment.NewLine)));
            }
        }

        private void txtEncodingCommand_TextChanged(object sender, EventArgs e)
        {

        }
    }
}