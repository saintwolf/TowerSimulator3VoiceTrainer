using System;
using System.IO;
using System.Linq; // Required for the 40-line safety check
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace TowerVoiceTrainer
{
    class MainForm : Form
    {
        private TextBox txtFilePath;
        private Button btnBrowse;
        private Button btnCustomTrain;
        private Button btnDefaultTrain;
        private Label lblStatus;

        public MainForm()
        {
            this.Text = "Tower Simulator 3 Voice Trainer";
            this.Size = new Size(420, 260);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            txtFilePath = new TextBox();
            txtFilePath.Location = new Point(20, 20);
            txtFilePath.Size = new Size(280, 20);
            txtFilePath.ReadOnly = true;
            this.Controls.Add(txtFilePath);

            btnBrowse = new Button();
            btnBrowse.Text = "Browse...";
            btnBrowse.Location = new Point(310, 18);
            btnBrowse.Click += BtnBrowse_Click;
            this.Controls.Add(btnBrowse);

            btnCustomTrain = new Button();
            btnCustomTrain.Text = "Launch Custom Voice Training (Needs File)";
            btnCustomTrain.Location = new Point(20, 60);
            btnCustomTrain.Size = new Size(365, 45);
            btnCustomTrain.Font = new Font(btnCustomTrain.Font, FontStyle.Bold);
            btnCustomTrain.Enabled = false; // Disabled until file is picked
            btnCustomTrain.Click += BtnCustomTrain_Click;
            this.Controls.Add(btnCustomTrain);

            btnDefaultTrain = new Button();
            btnDefaultTrain.Text = "Launch Default Windows Training";
            btnDefaultTrain.Location = new Point(20, 115);
            btnDefaultTrain.Size = new Size(365, 45);
            btnDefaultTrain.Font = new Font(btnDefaultTrain.Font, FontStyle.Regular);
            btnDefaultTrain.Click += BtnDefaultTrain_Click; // This one is always clickable
            this.Controls.Add(btnDefaultTrain);

            lblStatus = new Label();
            lblStatus.Text = "Select a .txt file with one simulator phrase per line.";
            lblStatus.Location = new Point(20, 175);
            lblStatus.Size = new Size(365, 20);
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblStatus);
        }

        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files (*.txt)|*.txt";
                ofd.Title = "Select your Tower Simulator Dictionary";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = ofd.FileName;
                    btnCustomTrain.Enabled = true;
                    lblStatus.Text = "File loaded. Ready for custom training.";
                    lblStatus.ForeColor = Color.Black;
                }
            }
        }

        private void BtnCustomTrain_Click(object? sender, EventArgs e)
        {
            if (!File.Exists(txtFilePath.Text)) return;

            var allLines = File.ReadAllLines(txtFilePath.Text);
            var validPhrases = allLines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

            if (validPhrases.Length > 40)
            {
                MessageBox.Show(
                    $"This file contains {validPhrases.Length} phrases. The Windows Speech engine will crash if you send more than 40 at a time.\n\nPlease split your text file into smaller batches.", 
                    "SAPI Limit Reached", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
                return; 
            }
            
            StringBuilder multiString = new StringBuilder();
            foreach (string phrase in validPhrases)
            {
                string cleanPhrase = phrase.Replace("/", " ").Replace("[", "").Replace("]", "").Trim();
                multiString.Append(cleanPhrase);
                multiString.Append('\0'); 
            }
            multiString.Append('\0');

            try
            {
                lblStatus.Text = "Launching Custom Speech Wizard...";
                lblStatus.ForeColor = Color.Blue;
                
                Type? sapiType = Type.GetTypeFromProgID("SAPI.SpSharedRecognizer");
                if (sapiType == null) throw new Exception("Speech API not found on this Windows install.");
                
                dynamic? recognizer = Activator.CreateInstance(sapiType);

                recognizer?.DisplayUI(this.Handle.ToInt32(), "Tower Simulator 3 Voice Trainer", "UserTraining", multiString.ToString());
                
                lblStatus.Text = "Custom training session finished!";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: Could not launch the Windows Speech UI.";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show(ex.Message, "SAPI API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDefaultTrain_Click(object? sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Launching Default Windows Speech Wizard...";
                lblStatus.ForeColor = Color.Blue;
                
                Type? sapiType = Type.GetTypeFromProgID("SAPI.SpSharedRecognizer");
                if (sapiType == null) throw new Exception("Speech API not found on this Windows install.");
                
                dynamic? recognizer = Activator.CreateInstance(sapiType);

                recognizer?.DisplayUI(this.Handle.ToInt32(), "Default Windows Voice Training", "UserTraining", "");
                
                lblStatus.Text = "Default training session finished!";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: Could not launch the Windows Speech UI.";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show(ex.Message, "SAPI API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}