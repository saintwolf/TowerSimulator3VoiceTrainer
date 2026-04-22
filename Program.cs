using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace TowerVoiceTrainer
{
    class MainForm : Form
    {
        private TextBox txtFilePath;
        private Button btnBrowse;
        private Button btnTrain;
        private Label lblStatus;

        public MainForm()
        {
            this.Text = "Tower Simulator 3 Voice Trainer";
            this.Size = new Size(420, 220);
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

            btnTrain = new Button();
            btnTrain.Text = "Launch SAPI Voice Training";
            btnTrain.Location = new Point(20, 60);
            btnTrain.Size = new Size(365, 50);
            btnTrain.Font = new Font(btnTrain.Font, FontStyle.Bold);
            btnTrain.Enabled = false;
            btnTrain.Click += BtnTrain_Click;
            this.Controls.Add(btnTrain);

            lblStatus = new Label();
            lblStatus.Text = "Select a .txt file with one simulator phrase per line.";
            lblStatus.Location = new Point(20, 130);
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
                    btnTrain.Enabled = true;
                    lblStatus.Text = "File loaded. Ready to train.";
                    lblStatus.ForeColor = Color.Black;
                }
            }
        }

        private void BtnTrain_Click(object? sender, EventArgs e)
        {
            if (!File.Exists(txtFilePath.Text)) return;

            string[] phrases = File.ReadAllLines(txtFilePath.Text);
            
            StringBuilder multiString = new StringBuilder();
            foreach (string phrase in phrases)
            {
                if (!string.IsNullOrWhiteSpace(phrase))
                {
                    multiString.Append(phrase.Trim());
                    multiString.Append('\0'); 
                }
            }
            multiString.Append('\0');

            try
            {
                lblStatus.Text = "Launching Windows Speech Wizard...";
                lblStatus.ForeColor = Color.Blue;
                
                Type? sapiType = Type.GetTypeFromProgID("SAPI.SpSharedRecognizer");
                if (sapiType == null) throw new Exception("Speech API not found on this Windows install.");
                
                dynamic? recognizer = Activator.CreateInstance(sapiType);

                recognizer?.DisplayUI(this.Handle.ToInt32(), "Tower Simulator 3 Voice Trainer", "UserTraining", multiString.ToString());
                
                lblStatus.Text = "Training session finished!";
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