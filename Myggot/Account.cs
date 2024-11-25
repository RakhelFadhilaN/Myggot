using Myggot;
using Myggot.Classes;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaggotInterface
{
    public partial class Account : Form
    {
        private LoggedInAccount loggedInAccount;
        private string connectionString;
        public Account(LoggedInAccount loggedInAccount)
        {
            InitializeComponent();
            this.Size = new Size(1920, 1080);
            this.loggedInAccount = loggedInAccount;
            connectionString = Program.connectionString;

            // Load the account details when the form initializes
            LoadAccountDetails();
        }

        // Method to load the account details into the labels
        private void LoadAccountDetails()
        {
            if (loggedInAccount == null || !loggedInAccount.IsLoggedIn || loggedInAccount.Account == null)
            {
                MessageBox.Show("No account information available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the logged-in account is a User
            if (loggedInAccount.IsUser())
            {
                User user = (User)loggedInAccount.Account;
                LblName.Text = user.Username;
                LabelStatus.Text = "User";
                label1.Text = user.Email;
            }
            // Check if the logged-in account is a Farmer
            else if (loggedInAccount.IsFarmer())
            {
                Farmer farmer = (Farmer)loggedInAccount.Account;
                LblName.Text = farmer.Username;  
                LabelStatus.Text = "Farmer"; 
                label1.Text = farmer.Email;     
            }
            else
            {
                MessageBox.Show("Unknown account type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LabelStatus_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void TBName_TextChanged(object sender, EventArgs e)
        {

        }

        private void TBEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void TBPhone_TextChanged(object sender, EventArgs e)
        {

        }

        private void TBAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void ButtonKeluar_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Hide();
        }

        private void LabelName_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string name = TBName.Text;
            string email = TBEmail.Text;
            string phone = TBPhone.Text;
            string address = TBAddress.Text;

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    // Prepare the base query and parameters
                    List<string> setClauses = new List<string>();
                    List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                    // Only update fields that are not empty
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        setClauses.Add("username = @name");
                        parameters.Add(new NpgsqlParameter("@name", name));
                    }
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        setClauses.Add("email = @email");
                        parameters.Add(new NpgsqlParameter("@email", email));
                    }
                    if (!string.IsNullOrWhiteSpace(phone))
                    {
                        setClauses.Add("telephone = @phone");
                        parameters.Add(new NpgsqlParameter("@phone", phone));
                    }
                    if (!string.IsNullOrWhiteSpace(address))
                    {
                        setClauses.Add("address = @address");
                        parameters.Add(new NpgsqlParameter("@address", address));
                    }

                    // If no fields were filled, return early
                    if (setClauses.Count == 0)
                    {
                        MessageBox.Show("No fields were changed.", "No Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Construct the query with dynamic SET clause
                    string query = string.Empty;
                    if (loggedInAccount.IsUser())
                    {
                        User user = (User)loggedInAccount.Account;
                        query = $"UPDATE users SET {string.Join(", ", setClauses)} WHERE id = @id";
                        parameters.Add(new NpgsqlParameter("@id", user.Id));
                    }
                    else if (loggedInAccount.IsFarmer())
                    {
                        Farmer farmer = (Farmer)loggedInAccount.Account;
                        query = $"UPDATE farmer SET {string.Join(", ", setClauses)} WHERE id = @id";
                        parameters.Add(new NpgsqlParameter("@id", farmer.Id));
                    }
                    else
                    {
                        MessageBox.Show("Unknown account type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Execute the query
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddRange(parameters.ToArray());

                        // Execute the update command
                        int rowsAffected = command.ExecuteNonQuery();
                        command.CommandTimeout = 60;
                        MessageBox.Show($"{rowsAffected} row(s) updated.", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display the error message if an exception occurs
                MessageBox.Show($"An error occurred while updating the database: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
    }
    
}



