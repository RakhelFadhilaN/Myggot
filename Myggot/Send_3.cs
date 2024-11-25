using Myggot;
using Myggot.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaggotInterface
{
    public partial class Send_3 : Form
    {
        private LoggedInAccount loggedInAccount;
        private string connectionString;
        public Send_3(LoggedInAccount loggedInAccount)
        {
            InitializeComponent();
            this.loggedInAccount = loggedInAccount;
            connectionString = Program.connectionString;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            Dashboard change = new Dashboard(loggedInAccount);

            change.Show();

            this.Hide();
        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            Send_1 change = new Send_1(loggedInAccount);

            change.Show();

            this.Hide();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton2_Click(object sender, EventArgs e)
        {
            Send_2 change = new Send_2(loggedInAccount);

            change.Show();

            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void IconDashboard_Click(object sender, EventArgs e)
        {
            Dashboard DashboardForm = new Dashboard(loggedInAccount);
            DashboardForm.Show();
            this.Hide();
            DashboardForm.UpdateTotalWaste();
        }

        private void IconSend_Click(object sender, EventArgs e)
        {
            if (loggedInAccount != null)
            {
                if (loggedInAccount.IsFarmer())
                {
                    // Open the form for farmers
                    Send_1F sendFarmer = new Send_1F(loggedInAccount);
                    sendFarmer.Show();
                }
                else
                {
                    // Open the form for users
                    Send_1 sendUser = new Send_1(loggedInAccount);
                    sendUser.Show();
                }

                // Hide the current Dashboard
                this.Hide();
            }
            else
            {
                MessageBox.Show("No account is logged in. Please log in first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void IconReward_Click(object sender, EventArgs e)
        {
            Rewards rew = new Rewards(loggedInAccount);
            rew.Show();
            this.Hide();
        }

        private void IconNotification_Click(object sender, EventArgs e)
        {
            Notification notification = new Notification(loggedInAccount);
            notification.Show();
            this.Hide();
        }

        private void IconAccount_Click(object sender, EventArgs e)
        {
            Account account = new Account(loggedInAccount);
            account.Show();
            this.Hide();
        }
    }
}
