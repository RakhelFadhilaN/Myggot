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
    public partial class Notification : Form
    {
        private LoggedInAccount loggedInAccount;
        private string connectionString;
        public Notification(LoggedInAccount loggedInAccount)
        {
            InitializeComponent();
            this.Size = new Size(1920, 1080);
            this.loggedInAccount = loggedInAccount;
            connectionString = Program.connectionString;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }
        

        private void IconDashboard_Click_1(object sender, EventArgs e) { 
            Dashboard DashboardForm = new Dashboard(loggedInAccount);
            DashboardForm.Show();
            this.Hide();
            DashboardForm.UpdateTotalWaste();
        }

        private void IconSend_Click(object sender, EventArgs e)
        {
            Send_1 send = new Send_1(loggedInAccount);
            send.Show();
            this.Hide();
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
        private void IconChat_Click(object sender, EventArgs e)
        {

        }
    }
}
