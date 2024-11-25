using Myggot;
using Myggot.Classes;
using Npgsql;
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
    public partial class Rewards : Form
    {

        private LoggedInAccount loggedInAccount;
        private string connectionString;
        public Rewards(LoggedInAccount loggedInAccount)
        {
            InitializeComponent();
            this.Size = new Size(1920, 1080);
            this.loggedInAccount = loggedInAccount;
            connectionString = Program.connectionString;
            DisplayTotalPoints();
        }

        private void alamat_Click(object sender, EventArgs e)
        {

        }

        private void ButtonTukar_Click(object sender, EventArgs e)
        {

        }

        private void Diamonds_Click(object sender, EventArgs e)
        {

        }

        private void HeaderVoucher_Click(object sender, EventArgs e)
        {

        }

        private void KeteranganVoucher_Click(object sender, EventArgs e)
        {

        }

        private void DisplayTotalPoints()
        {
            // Query to get totalpoints for the logged-in user
            string query = "SELECT totalpoints FROM users WHERE username = @username";

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        // Add parameter to avoid SQL injection
                        User user = (User)loggedInAccount.Account;
                        command.Parameters.AddWithValue("@username", user.Username);

                        // Execute the query and fetch totalpoints
                        var result = command.ExecuteScalar();

                        if (result != null)
                        {
                            int totalPoints = Convert.ToInt32(result);
                            Diamonds.Text = $"Total Points: {totalPoints} Diamond"; // Assuming Diamonds is a Label
                        }
                        else
                        {
                            Diamonds.Text = "Total Points: 0"; // If no result is found, show 0
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching total points: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void IconDashboard_Click_1(object sender, EventArgs e)
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
        private void Navbar_Click(object sender, EventArgs e)
        {

        }
    }
}
