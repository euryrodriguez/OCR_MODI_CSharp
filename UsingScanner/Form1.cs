using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WIA;
using MODI;
using System.IO;

namespace UsingScanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void scan_button_Click(object sender, EventArgs e)
        {
            // Create a DeviceManager instance
            var deviceManager = new DeviceManager();

            // Create an empty variable to store the scanner instance
            DeviceInfo firstScannerAvailable = null;

            // Loop through the list of devices to choose the first available
            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                // Skip the device if it's not a scanner
                if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                {
                    continue;
                }

                firstScannerAvailable = deviceManager.DeviceInfos[i];

                break;
            }

            // Connect to the first available scanner
            var device = firstScannerAvailable.Connect();

            // Select the scanner
            var scannerItem = device.Items[1];

            // Retrieve a image in JPEG format and store it into a variable
            var imageFile = (ImageFile)scannerItem.Transfer(FormatID.wiaFormatPNG);

            // Save the image in some path with filename
            var path = @"./img/scan.png";

            if (File.Exists(path))
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                File.Delete(path);
            }

            // Save image !
            imageFile.SaveFile(path);

            Bitmap imagen = new Bitmap(path);

            //  pictureBox1.Image = (System.Drawing.Image) imagen;
            //I did this because there was a process using the file , so the image could not be deleted

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                pictureBox1.Image = System.Drawing.Image.FromStream(stream);
                stream.Dispose();
            }

            Document MyVariable = new Document();
            MyVariable.Create(path);
            MyVariable.OCR(MODI.MiLANGUAGES.miLANG_SPANISH, true, true);
            MODI.Image image2 = (MODI.Image)MyVariable.Images[0];
            Layout layout = image2.Layout;
            string str = "";

            foreach (Word word in layout.Words)
                str += word.Text + " ";

            richTextBox1.Text = str;
            fillRegistroForm(str);
        }

        public void fillRegistroForm(string data) {
            Form registro = new Registro();
            string nombre = getBetween(data, "perteneciente a:", "De sexo");
            string padre = getBetween(data, "DECLARA", "quien es el padre");
            string madre = getBetween(data, "MADRE", ", p");
            string[] padreArr = padre.Split(' ');
            string lugar_de_nacimiento = getBetween(data, "nacido en", "el día");
            string Sexo = getBetween(data, "sexo", "nacido en");
            Sexo = Sexo.Replace(',', ' ').Trim();

            foreach (Control c in registro.Controls)
            {
                if (c is TextBox) {
                    if (c.Name == "textBoxNombre")
                        c.Text = nombre;
                    else if (c.Name == "textBoxLugarNacimiento")
                        c.Text = lugar_de_nacimiento;
                    else if (c.Name == "textBoxApellido")
                        c.Text = padreArr[1];
                    else if (c.Name == "textBoxPadre")
                        c.Text = padre;
                    else if (c.Name == "textBoxMadre")
                        c.Text = madre;
                    else if (c.Name == "textBoxSexo")
                        c.Text = Sexo;

                }
            }

            registro.Show();
        }

        public string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        public static string RemoveSpecialCharacters(string str)
        {
            char[] buffer = new char[str.Length];
            int idx = 0;

            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z')
                    || (c >= 'a' && c <= 'z') || (c == '.') || (c == '_'))
                {
                    buffer[idx] = c;
                    idx++;
                }
            }

            return new string(buffer, 0, idx);
        }

        public static string cleanString(string str){

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsLetterOrDigit(str[i]))
                {
                    sb.Append(str[i]);
                }
            }

            return sb.ToString();
        }
    }
}
