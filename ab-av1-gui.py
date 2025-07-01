import tkinter as tk
from tkinter import ttk, filedialog, scrolledtext, messagebox
import subprocess
import threading
import queue
import os
from datetime import datetime

class AB_AV1_GUI(tk.Tk):
    """
    A modern and simplified GUI for the AB-AV1 encoder.
    """
    def __init__(self):
        super().__init__()

        # --- Window Setup ---
        self.title("AB-AV1 Encoder GUI")
        self.geometry("800x650")
        self.minsize(600, 500)

        # --- State Variables ---
        self.encoding_queue = queue.Queue()
        self.is_encoding = False
        self.current_process = None
        self.cancellation_token = threading.Event()

        # --- UI Initialization ---
        self._setup_styles()
        self._create_widgets()

    def _setup_styles(self):
        """Configures styles for ttk widgets for a modern look."""
        style = ttk.Style(self)
        style.theme_use('clam')
        style.configure("TButton", padding=6, relief="flat", background="#0078D7", foreground="white")
        style.map("TButton",
            foreground=[('pressed', 'white'), ('active', 'white')],
            background=[('pressed', '!disabled', '#005A9E'), ('active', '#005A9E')]
        )
        style.configure("Stop.TButton", background="#DA3B01", foreground="white")
        style.map("Stop.TButton",
            background=[('pressed', '!disabled', '#A4262C'), ('active', '#A4262C')]
        )
        style.configure("TProgressbar", thickness=15)

    def _create_widgets(self):
        """Creates and lays out all the widgets in the window."""
        self.columnconfigure(0, weight=1)
        self.rowconfigure(2, weight=1) # Queue listbox
        self.rowconfigure(4, weight=1) # Log area

        # --- Command Frame ---
        command_frame = ttk.Frame(self, padding="10")
        command_frame.grid(row=0, column=0, sticky="ew", padx=10, pady=(10, 0))
        command_frame.columnconfigure(0, weight=1)

        ttk.Label(command_frame, text="Encoding Command Template:", font="-weight bold").grid(row=0, column=0, sticky="w")
        self.command_text = tk.Text(command_frame, height=4, wrap="word", relief="solid", borderwidth=1)
        self.command_text.grid(row=1, column=0, sticky="ew")
        default_command = "auto-encode --encoder h264_nvenc --input \"{filePath}\" --preset slow --pix-format yuv420p --keyint 120 -o \"{outputFile}\""
        self.command_text.insert("1.0", default_command)
        
        # --- Queue Frame ---
        queue_frame = ttk.Frame(self, padding="10")
        queue_frame.grid(row=1, column=0, sticky="ew", padx=10)
        queue_frame.columnconfigure(1, weight=1)

        ttk.Label(queue_frame, text="Encoding Queue:", font="-weight bold").grid(row=0, column=0, columnspan=3, sticky="w", pady=(0, 5))
        
        self.add_files_button = ttk.Button(queue_frame, text="Add Files...", command=self.add_files)
        self.add_files_button.grid(row=1, column=0, sticky="w")

        self.remove_file_button = ttk.Button(queue_frame, text="Remove Selected", command=self.remove_selected)
        self.remove_file_button.grid(row=1, column=1, sticky="w", padx=5)

        # --- Listbox for the queue ---
        listbox_frame = ttk.Frame(self)
        listbox_frame.grid(row=2, column=0, sticky="nsew", padx=10, pady=5)
        listbox_frame.rowconfigure(0, weight=1)
        listbox_frame.columnconfigure(0, weight=1)
        
        self.queue_listbox = tk.Listbox(listbox_frame, selectmode=tk.EXTENDED)
        self.queue_listbox.grid(row=0, column=0, sticky="nsew")
        
        scrollbar = ttk.Scrollbar(listbox_frame, orient="vertical", command=self.queue_listbox.yview)
        scrollbar.grid(row=0, column=1, sticky="ns")
        self.queue_listbox.config(yscrollcommand=scrollbar.set)

        # --- Progress and Controls Frame ---
        progress_frame = ttk.Frame(self, padding="10")
        progress_frame.grid(row=3, column=0, sticky="ew", padx=10, pady=5)
        progress_frame.columnconfigure(0, weight=1)

        self.progress_bar = ttk.Progressbar(progress_frame, orient="horizontal", mode="determinate")
        self.progress_bar.grid(row=0, column=0, sticky="ew", padx=(0, 10))

        self.start_button = ttk.Button(progress_frame, text="Start Encoding", command=self.start_encoding)
        self.start_button.grid(row=0, column=1, padx=(0, 5))

        self.stop_button = ttk.Button(progress_frame, text="Stop Encoding", style="Stop.TButton", state="disabled", command=self.stop_encoding)
        self.stop_button.grid(row=0, column=2)

        # --- Log Frame ---
        log_frame = ttk.Frame(self, padding="10")
        log_frame.grid(row=4, column=0, sticky="nsew", padx=10, pady=(0, 10))
        log_frame.rowconfigure(1, weight=1)
        log_frame.columnconfigure(0, weight=1)

        ttk.Label(log_frame, text="Log:", font="-weight bold").grid(row=0, column=0, sticky="w", pady=(0, 5))
        self.log_area = scrolledtext.ScrolledText(log_frame, state="disabled", wrap="word")
        self.log_area.grid(row=1, column=0, sticky="nsew")

    def _log(self, message):
        """Thread-safe logging to the text area."""
        def append():
            self.log_area.config(state="normal")
            self.log_area.insert(tk.END, message + "\n")
            self.log_area.config(state="disabled")
            self.log_area.see(tk.END)
        self.after(0, append)

    def add_files(self):
        """Opens a file dialog to add video files to the queue."""
        if self.is_encoding:
            messagebox.showwarning("Encoding in Progress", "Cannot add files while encoding is active.")
            return
        
        filetypes = [("Video Files", "*.mp4 *.mkv *.avi *.mov *.webm"), ("All files", "*.*")]
        files = filedialog.askopenfilenames(title="Select Video Files", filetypes=filetypes)
        for file in files:
            self.queue_listbox.insert(tk.END, file)

    def remove_selected(self):
        """Removes selected files from the queue listbox."""
        if self.is_encoding:
            messagebox.showwarning("Encoding in Progress", "Cannot remove files while encoding is active.")
            return
            
        selected_indices = self.queue_listbox.curselection()
        # Iterate backwards to avoid index shifting issues
        for i in reversed(selected_indices):
            self.queue_listbox.delete(i)

    def start_encoding(self):
        """Starts the encoding process in a separate thread."""
        if self.is_encoding:
            self._log("ERROR: Encoding is already in progress.")
            return

        if self.queue_listbox.size() == 0:
            messagebox.showerror("Queue Empty", "Please add files to the queue before starting.")
            return

        # --- Prepare for encoding ---
        self.is_encoding = True
        self.cancellation_token.clear()
        self.log_area.config(state="normal")
        self.log_area.delete("1.0", tk.END)
        self.log_area.config(state="disabled")

        # Populate internal queue from listbox
        for i in range(self.queue_listbox.size()):
            self.encoding_queue.put(self.queue_listbox.get(i))
        
        self.progress_bar['maximum'] = self.queue_listbox.size()
        self.progress_bar['value'] = 0

        # Update UI state
        self.start_button.config(state="disabled")
        self.stop_button.config(state="normal")
        self.add_files_button.config(state="disabled")
        self.remove_file_button.config(state="disabled")

        # Start the worker thread
        self.encoding_thread = threading.Thread(target=self.encoding_worker, daemon=True)
        self.encoding_thread.start()

    def stop_encoding(self):
        """Signals the encoding thread to stop."""
        if not self.is_encoding:
            return
        
        self._log("--- CANCELLATION REQUESTED ---")
        self.cancellation_token.set()
        if self.current_process:
            try:
                self._log(f"Terminating current process (PID: {self.current_process.pid})...")
                self.current_process.kill()
            except Exception as e:
                self._log(f"Error terminating process: {e}")
        
        self.stop_button.config(state="disabled")

    def encoding_worker(self):
        """The main worker function that processes the queue."""
        files_processed = 0
        while not self.encoding_queue.empty():
            if self.cancellation_token.is_set():
                self._log("Encoding cancelled by user.")
                break
            
            input_path = self.encoding_queue.get()
            self._log("="*60)
            self._log(f"Starting file {files_processed + 1} of {self.progress_bar['maximum']}: {os.path.basename(input_path)}")
            
            # --- Generate output path ---
            try:
                directory = os.path.dirname(input_path)
                filename, ext = os.path.splitext(os.path.basename(input_path))
                timestamp = datetime.now().strftime('%Y%m%d-%H%M%S')
                output_path = os.path.join(directory, f"{filename}_{timestamp}{ext}")
            except Exception as e:
                self._log(f"ERROR: Could not generate output path. Skipping. Details: {e}")
                continue

            # --- Prepare and run command ---
            command_template = self.command_text.get("1.0", tk.END).strip()
            command = command_template.replace("{filePath}", f'"{input_path}"')
            command = command.replace("{outputFile}", f'"{output_path}"')
            
            self._log(f"Executing command: {command}")

            try:
                # Use CREATE_NO_WINDOW on Windows to hide the console
                creationflags = subprocess.CREATE_NO_WINDOW if os.name == 'nt' else 0
                self.current_process = subprocess.Popen(
                    command,
                    shell=True,
                    stdout=subprocess.PIPE,
                    stderr=subprocess.STDOUT,
                    text=True,
                    encoding='utf-8',
                    errors='replace',
                    creationflags=creationflags
                )

                # Read output in real-time
                for line in self.current_process.stdout:
                    self._log(line.strip())
                
                self.current_process.wait()
                return_code = self.current_process.returncode
                self.current_process = None

                if self.cancellation_token.is_set():
                    self._log(f"Process for {os.path.basename(input_path)} was terminated.")
                    break

                if return_code == 0:
                    self._log(f"SUCCESS: Finished encoding {os.path.basename(input_path)}")
                else:
                    self._log(f"ERROR: Process for {os.path.basename(input_path)} failed with exit code {return_code}.")

            except FileNotFoundError:
                self._log("FATAL ERROR: 'auto-encode' command not found. Is AB-AV1 installed and in your system's PATH?")
                break # Stop the entire queue if the command is missing
            except Exception as e:
                self._log(f"An unexpected error occurred: {e}")
            
            files_processed += 1
            self.after(0, lambda: self.progress_bar.config(value=files_processed))

        # --- Final cleanup ---
        self.after(0, self.on_encoding_finished)

    def on_encoding_finished(self):
        """Resets the UI to its initial state after encoding is done or cancelled."""
        if self.cancellation_token.is_set():
            self._log("\n--- ENCODING CANCELLED ---")
        else:
            self._log("\n--- ENCODING QUEUE FINISHED ---")
            messagebox.showinfo("Complete", "All files in the queue have been processed.")
        
        self.is_encoding = False
        self.current_process = None
        self.cancellation_token.clear()

        # Clear the internal queue
        while not self.encoding_queue.empty():
            try:
                self.encoding_queue.get_nowait()
            except queue.Empty:
                break
        
        # Reset UI elements
        self.start_button.config(state="normal")
        self.stop_button.config(state="disabled")
        self.add_files_button.config(state="normal")
        self.remove_file_button.config(state="normal")
        self.progress_bar['value'] = 0


if __name__ == "__main__":
    app = AB_AV1_GUI()
    app.mainloop()

