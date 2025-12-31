using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility; 

namespace aadharQR
{
    public partial class Form1 : Form
    {
        // UI Components
        private Button btnUpload;
        private PictureBox picQR;
        private Label lblName, lblGender, lblDOB, lblAddress;
        private TextBox txtName, txtGender, txtDOB, txtAddress;
        private Label lblTitle;

        public Form1()
        {
            
            InitializeCustomComponents();

            this.Text = "Aadhaar QR Reader";
            this.Size = new Size(500, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void InitializeCustomComponents()
        {
            lblTitle = new Label() { Text = "Aadhaar Detail Extractor", Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 20), Size = new Size(400, 40), ForeColor = Color.DarkBlue };

            btnUpload = new Button() { Text = "Upload QR Image", Location = new Point(20, 70), Size = new Size(440, 40), BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnUpload.Click += BtnUpload_Click;

            picQR = new PictureBox() { Location = new Point(20, 120), Size = new Size(440, 200), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.White };

            // Input Fields
            int startY = 340;
            int spacing = 60;

            lblName = CreateLabel("Full Name:", 20, startY);
            txtName = CreateTextBox(20, startY + 20);
            lblGender = CreateLabel("Gender:", 20, startY + spacing);
            txtGender = CreateTextBox(20, startY + spacing + 20);
            lblDOB = CreateLabel("Date of Birth:", 20, startY + (spacing * 2));
            txtDOB = CreateTextBox(20, startY + (spacing * 2) + 20);
            lblAddress = CreateLabel("Address:", 20, startY + (spacing * 3));
            txtAddress = CreateTextBox(20, startY + (spacing * 3) + 20, true);
            txtAddress.Height = 80;

            this.Controls.AddRange(new Control[] { lblTitle, btnUpload, picQR, lblName, txtName, lblGender, txtGender, lblDOB, txtDOB, lblAddress, txtAddress });
        }

        private Label CreateLabel(string text, int x, int y) => new Label() { Text = text, Location = new Point(x, y), AutoSize = true };
        private TextBox CreateTextBox(int x, int y, bool multiLine = false) => new TextBox() { Location = new Point(x, y), Width = 440, ReadOnly = true, Multiline = multiLine, BackColor = Color.White };

        private void BtnUpload_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Images|*.jpg;*.png;*.jpeg;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bitmap = new Bitmap(ofd.FileName);
                    picQR.Image = bitmap;

                   
                    var reader = new BarcodeReader();
                    var result = reader.Decode(bitmap);

                    if (result != null)
                        ParseAadhaarData(result.Text);
                    else
                        MessageBox.Show("QR Code not detected.");
                }
            }
        }

        private void ParseAadhaarData(string data)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(data);
                XmlNode? node = xmlDoc.SelectSingleNode("PrintLetterBarcodeData");

                if (node != null)
                {
                    txtName.Text = node.Attributes?["name"]?.Value;
                    txtGender.Text = node.Attributes?["gender"]?.Value;
                    txtDOB.Text = node.Attributes?["dob"]?.Value;
                    txtAddress.Text = $"{node.Attributes?["house"]?.Value}, {node.Attributes?["street"]?.Value}, {node.Attributes?["vtc"]?.Value}";
                }
            }
            catch
            {
                txtAddress.Text = "Raw Data: " + data;
            }
        }
    }
}
