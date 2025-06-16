using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using ab_av1_gui.Properties;
using System.Linq; // Added for .Cast<string>()
using System.Collections.Generic; // Added for List<Task>

namespace ab_av1_gui
{
    public partial class MainForm : Form
    {
        private ConcurrentDictionary<Process, string> activeEncodingProcesses = new ConcurrentDictionary<Process, string>();
        private SemaphoreSlim encodingSemaphore;
        private ConcurrentQueue<string> filesToEncode = new ConcurrentQueue<string>();
        private CancellationTokenSource encodingCancellationTokenSource;

        public MainForm()
        {
            InitializeComponent();
            txtEncodingCommand.Text = "auto-encode --encoder h264_nvenc --input {filePath} --preset slow --pix-format yuv420p --keyint 120 -o {outputFile}";
            // Initialize semaphore with a default, it will be updated on form load
            encodingSemaphore = new SemaphoreSlim(1, 1);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Load dark mode setting
            chkDarkMode.Checked = Settings.Default.DarkModeEnabled;
            ApplyTheme(chkDarkMode.Checked);

            // Load max concurrent encodes setting, default to 1 if not set
            numericMaxConcurrentEncodes.Value = Settings.Default.MaxConcurrentEncodes > 0 ? Settings.Default.MaxConcurrentEncodes : 1;
            UpdateSemaphoreCount((int)numericMaxConcurrentEncodes.Value); // Update semaphore based on loaded setting

            // Set the maximum value of numericCpuCoreLimit to the number of logical processors on the system
            numericCpuCoreLimit.Maximum = Environment.ProcessorCount;

            // Load CPU core limit setting, default to Environment.ProcessorCount if not set or invalid
            int defaultCpuCores = Environment.ProcessorCount;
            if (Settings.Default.LimitCpuCores > 0 && Settings.Default.LimitCpuCores <= numericCpuCoreLimit.Maximum)
            {
                numericCpuCoreLimit.Value = Settings.Default.LimitCpuCores;
            }
            else
            {
                numericCpuCoreLimit.Value = defaultCpuCores;
                Settings.Default.LimitCpuCores = defaultCpuCores; // Save default if it's applied
                Settings.Default.Save();
            }
        }

        private void chkDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            ApplyTheme(chkDarkMode.Checked);
            Settings.Default.DarkModeEnabled = chkDarkMode.Checked;
            Settings.Default.Save();
        }

        private void numericMaxConcurrentEncodes_ValueChanged(object sender, EventArgs e)
        {
            UpdateSemaphoreCount((int)numericMaxConcurrentEncodes.Value);
            Settings.Default.MaxConcurrentEncodes = (int)numericMaxConcurrentEncodes.Value;
            Settings.Default.Save();
        }

        private void numericCpuCoreLimit_ValueChanged(object sender, EventArgs e)
        {
            // Save the new value to settings
            Settings.Default.LimitCpuCores = (int)numericCpuCoreLimit.Value;
            Settings.Default.Save();
        }

        private void UpdateSemaphoreCount(int newMaxCount)
        {
            // Dispose of the old semaphore if it exists to properly reinitialize
            if (encodingSemaphore != null)
            {
                encodingSemaphore.Dispose();
            }
            encodingSemaphore = new SemaphoreSlim(newMaxCount, newMaxCount);
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
                        listBoxQueue.Items.Add(file);
                    }
                }
            }
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

            BtnStartEncoding.Enabled = false; // Disable start button during encoding
            BtnStopEncoding.Enabled = true;   // Enable stop button

            progressBar1.Value = 0;
            progressBar1.Maximum = listBoxQueue.Items.Count;

            filesToEncode = new ConcurrentQueue<string>(listBoxQueue.Items.Cast<string>());
            listBoxQueue.Items.Clear(); // Clear the visible listbox as items move to processing queue

            encodingCancellationTokenSource = new CancellationTokenSource();

            var encodingTasks = new List<Task>();
            foreach (string filePath in filesToEncode)
            {
                // Capture filePath for use in the lambda
                string currentFilePath = filePath;
                encodingTasks.Add(Task.Run(async () =>
                {
                    // Acquire a slot from the semaphore
                    await encodingSemaphore.WaitAsync(encodingCancellationTokenSource.Token);
                    try
                    {
                        if (encodingCancellationTokenSource.Token.IsCancellationRequested) return; // Check for cancellation

                        await Task.Run(() => EncodeFile(currentFilePath, encodingCancellationTokenSource.Token), encodingCancellationTokenSource.Token);
                    }
                    finally
                    {
                        encodingSemaphore.Release(); // Release the slot
                    }
                }, encodingCancellationTokenSource.Token));
            }

            try
            {
                await Task.WhenAll(encodingTasks);
                AppendLog("All encoding tasks completed.");
            }
            catch (OperationCanceledException)
            {
                AppendLog("Encoding process was cancelled.");
            }
            catch (Exception ex)
            {
                AppendLog($"An error occurred during encoding: {ex.Message}");
            }
            finally
            {
                BtnStartEncoding.Enabled = true; // Re-enable start button
                BtnStopEncoding.Enabled = false; // Disable stop button
                encodingCancellationTokenSource?.Dispose();
                encodingCancellationTokenSource = null;
            }
        }

        private void BtnStopEncoding_Click(object sender, EventArgs e)
        {
            if (encodingCancellationTokenSource != null && !encodingCancellationTokenSource.IsCancellationRequested)
            {
                encodingCancellationTokenSource.Cancel(); // Request cancellation
                AppendLog("Attempting to stop all encoding processes...");

                // Also kill any active processes immediately
                foreach (var entry in activeEncodingProcesses)
                {
                    Process proc = entry.Key;
                    if (proc != null && !proc.HasExited)
                    {
                        try
                        {
                            proc.Kill();
                            proc.Dispose();
                            AppendLog($"Killed process for {Path.GetFileName(entry.Value)}.");
                        }
                        catch (InvalidOperationException)
                        {
                            // Process might have already exited or is being disposed
                        }
                    }
                }
                activeEncodingProcesses.Clear(); // Clear the dictionary

                // Reset UI elements
                BtnStartEncoding.Enabled = true;
                BtnStopEncoding.Enabled = false;
                progressBar1.Value = 0; // Or keep current progress
            }
        }

        private void EncodeFile(string filePath, CancellationToken cancellationToken)
        {
            // Correctly quote paths as before
            string escapedFilePath = $"\"{filePath}\"";
            string outputDirectory = Path.GetDirectoryName(filePath);
            string outputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string outputFilePath = Path.Combine(outputDirectory, outputFileNameWithoutExtension + "_encoded.mkv");
            string escapedOutputFile = $"\"{outputFilePath}\"";

            string commandArguments = txtEncodingCommand.Text
                .Replace("{filePath}", escapedFilePath)
                .Replace("{outputFile}", escapedOutputFile);

            // Removed the --threads argument as it's not supported by ab-av1.exe

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "ab-av1.exe",
                Arguments = commandArguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process currentProcess = new Process { StartInfo = psi };

            // Add the process and its input file to the dictionary
            activeEncodingProcesses.TryAdd(currentProcess, filePath);

            currentProcess.OutputDataReceived += (s, e) => AppendLog(e.Data);
            currentProcess.ErrorDataReceived += (s, e) => AppendLog(e.Data);

            int processExitCode = -1; // Default to a non-zero value indicating failure or not yet exited

            try
            {
                currentProcess.Start();

                // --- Apply Processor Affinity ---
                // Get the user-selected number of cores to limit to
                int coresToLimitTo = (int)numericCpuCoreLimit.Value;
                if (coresToLimitTo > 0 && coresToLimitTo <= Environment.ProcessorCount)
                {
                    // Create a bitmask for the first 'coresToLimitTo' logical processors
                    long affinityMask = (1L << coresToLimitTo) - 1;
                    try
                    {
                        currentProcess.ProcessorAffinity = (IntPtr)affinityMask;
                        AppendLog($"Set processor affinity for {Path.GetFileName(filePath)} to {coresToLimitTo} cores.");
                    }
                    catch (InvalidOperationException ex)
                    {
                        AppendLog($"Warning: Could not set processor affinity for {Path.GetFileName(filePath)}. Error: {ex.Message}");
                    }
                    catch (System.ComponentModel.Win32Exception ex)
                    {
                        // Handle cases where the affinity cannot be set due to permissions or OS issues
                        AppendLog($"Warning: Win32Exception when setting processor affinity for {Path.GetFileName(filePath)}. Error: {ex.Message}");
                    }
                }
                // --- End Processor Affinity ---

                currentProcess.BeginOutputReadLine();
                currentProcess.BeginErrorReadLine();

                // Wait for the process to exit or for cancellation
                currentProcess.WaitForExit();

                processExitCode = currentProcess.ExitCode; // Capture ExitCode immediately after exit

                cancellationToken.ThrowIfCancellationRequested(); // Check cancellation after process exits
            }
            catch (OperationCanceledException)
            {
                AppendLog($"Encoding for {Path.GetFileName(filePath)} cancelled.");
                if (!currentProcess.HasExited)
                {
                    currentProcess.Kill(); // Ensure it's killed if cancellation happened mid-process
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Error encoding {Path.GetFileName(filePath)}: {ex.Message}");
            }
            finally
            {
                // Remove the process from the active list
                activeEncodingProcesses.TryRemove(currentProcess, out _);
                // Pass the captured exit code and output file path to the UI thread for final processing
                Invoke(new Action(() => OnEncodingFinished(processExitCode, filePath, outputFilePath)));
                currentProcess.Dispose(); // Dispose the process object here
            }
        }

        // Modified OnEncodingFinished to receive exit code directly and output file path
        private void OnEncodingFinished(int exitCode, string originalFilePath, string outputFilePath)
        {
            if (progressBar1.Value < progressBar1.Maximum)
                progressBar1.Value += 1;

            // --- Conditional File Deletion Logic ---
            if (chkDeleteOriginal.Checked)
            {
                // Check if process exited successfully (exit code 0)
                if (exitCode == 0)
                {
                    // Check if output file exists and is not empty
                    if (File.Exists(outputFilePath))
                    {
                        FileInfo fi = new FileInfo(outputFilePath);
                        if (fi.Length > 0) // Check if file size is greater than 0 bytes
                        {
                            try
                            {
                                File.Delete(originalFilePath);
                                AppendLog($"Successfully encoded and deleted original file: {Path.GetFileName(originalFilePath)}");
                            }
                            catch (Exception ex)
                            {
                                AppendLog($"Error deleting original file {Path.GetFileName(originalFilePath)}: {ex.Message}");
                            }
                        }
                        else
                        {
                            AppendLog($"Output file {Path.GetFileName(outputFilePath)} is empty. Not deleting original: {Path.GetFileName(originalFilePath)}");
                        }
                    }
                    else
                    {
                        AppendLog($"Output file {Path.GetFileName(outputFilePath)} not found. Not deleting original: {Path.GetFileName(originalFilePath)}");
                    }
                }
                else
                {
                    AppendLog($"Encoding for {Path.GetFileName(originalFilePath)} failed (Exit Code: {exitCode}). Not deleting original file.");
                }
            }
        }

        private void AppendLog(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Invoke(new Action(() =>
                {
                    txtLog.AppendText(text + Environment.NewLine);
                    // Auto-scroll to the bottom
                    txtLog.SelectionStart = txtLog.Text.Length;
                    txtLog.ScrollToCaret();
                }));
            }
        }

        private void txtEncodingCommand_TextChanged(object sender, EventArgs e)
        {
            // No changes needed here for dark mode
        }

        private void ApplyTheme(bool isDarkMode)
        {
            Color backColor;
            Color foreColor;
            Color textBoxBackColor;
            Color textBoxForeColor;

            if (isDarkMode)
            {
                backColor = Color.FromArgb(30, 30, 30); // Dark gray for background
                foreColor = Color.White; // White for text
                textBoxBackColor = Color.FromArgb(45, 45, 48); // Slightly lighter dark for text boxes
                textBoxForeColor = Color.White; // White text in text boxes
            }
            else
            {
                backColor = SystemColors.Control; // Default light gray
                foreColor = SystemColors.ControlText; // Default black text
                textBoxBackColor = SystemColors.Window; // Default white
                textBoxForeColor = SystemColors.WindowText; // Default black
            }

            // Apply colors to the form
            this.BackColor = backColor;
            this.ForeColor = foreColor;

            // Apply colors to all controls on the form
            foreach (Control control in this.Controls)
            {
                ApplyControlTheme(control, backColor, foreColor, textBoxBackColor, textBoxForeColor);
            }
        }

        private void ApplyControlTheme(Control control, Color backColor, Color foreColor, Color textBoxBackColor, Color textBoxForeColor)
        {
            if (control is Button)
            {
                control.BackColor = backColor;
                control.ForeColor = foreColor;
            }
            // Included NumericUpDown here to apply textbox colors
            else if (control is TextBox || control is ListBox || control is RichTextBox || control is NumericUpDown)
            {
                control.BackColor = textBoxBackColor;
                control.ForeColor = textBoxForeColor;
            }
            else if (control is Label || control is CheckBox)
            {
                control.BackColor = backColor;
                control.ForeColor = foreColor;
            }
            // Add more control types as needed (e.g., DataGridView, ComboBox)
            // You might need more specific handling for complex controls

            // Recursively apply to child controls (e.g., if you have GroupBoxes)
            foreach (Control childControl in control.Controls)
            {
                ApplyControlTheme(childControl, backColor, foreColor, textBoxBackColor, textBoxForeColor);
            }
        }
    }
}