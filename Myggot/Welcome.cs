using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Myggot;

namespace MaggotInterface
{
    public partial class Welcome : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
            (
            int nLeft,
            int nTop,
            int nRight,
            int nBottom, 
            int nWidthEllipse,
            int nHeightEllipse);
        public Welcome()
        {
            InitializeComponent();
            this.Size = new Size(1920, 1080);
        }

    

        private void Form1_Load(object sender, EventArgs e)
        {
            BtnMasuk.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, BtnMasuk.Width, BtnMasuk.Height, 30, 30));
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Open the Login form
            Login loginForm = new Login();
            loginForm.Show(); // Opens the form
            this.Hide();      // Optionally hide the current form
        }

        private void label5_Click(object sender, EventArgs e)
        {
            // Open the Register form
            Register registerForm = new Register();
            registerForm.Show(); // Opens the form
            this.Hide();         // Optionally hide the current form
        }

    }
}
