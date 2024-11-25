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
using Myggot.Classes;
using DotNetEnv;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MaggotInterface;


namespace Myggot
{
    public partial class Login : Form
    {

        private string connectionString;

        private LoggedInAccount loggedInAccount;

        private AccountCache accountCache = new AccountCache();
        public Login()
        {
            InitializeComponent();
            this.Size = new Size(1920, 1080);
            connectionString = Program.connectionString;

            comboBox1.Items.Add("Pemilik Sampah");
            comboBox1.Items.Add("Petani");
            loggedInAccount = new LoggedInAccount();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string selectedRole = comboBox1.SelectedItem?.ToString()?.Trim().Normalize(NormalizationForm.FormC);
            if (string.IsNullOrEmpty(selectedRole))
            {
                MessageBox.Show("Pilih peran Anda.", "Login gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IRegistrable person;
            if (string.Equals(selectedRole, "Pemilik Sampah", StringComparison.OrdinalIgnoreCase))
            {
                person = new User();
            }
            else if (string.Equals(selectedRole, "Petani", StringComparison.OrdinalIgnoreCase))
            {
                person = new Farmer();
            }
            else
            {
                MessageBox.Show("Peran tidak valid.", "Login gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            person.Username = textBox1.Text;
            person.Password = textBox2.Text; // Raw password; will be hashed

            if (IsLoginValid(person, selectedRole))
            {
                loggedInAccount.SetLoggedInAccount(person);
                MessageBox.Show($"Selamat datang, {person.Username}!", "Login berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Dashboard dashboardForm = new Dashboard(loggedInAccount);
                dashboardForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Coba cek lagi kredensialnya dan coba lagi.", "Login gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }









        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Register registrationForm = new Register();
            registrationForm.Show();
            this.Hide();
        }

        private void ShowNotification(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string HashPassword(string password)
        {
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }


        private bool IsLoginValid(IRegistrable person, string role)
        {
            if (string.IsNullOrEmpty(person.Username) || string.IsNullOrEmpty(person.Password))
            {
                return false;
            }

            string hashedPassword = HashPassword(person.Password);

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string tableName = role == "Pemilik Sampah" ? "users" : "farmer";
                    string query = $"SELECT id, username, totalwaste FROM {tableName} WHERE username = @username AND password = @password";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("username", person.Username);
                        cmd.Parameters.AddWithValue("password", hashedPassword);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int accountId = reader.GetInt32(0);
                                string username = reader.GetString(1);

                                if (role == "Pemilik Sampah" && person is User user)
                                {
                                    user.Id = accountId;
                                    accountCache.AddToCache(accountId, "totalwaste", reader.GetInt32(2));
                                }
                                else if (role == "Petani" && person is Farmer farmer)
                                {
                                    farmer.Id = accountId;
                                    accountCache.AddToCache(accountId, "totalwaste", reader.GetInt32(2));
                                }

                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification("Error", $"An error occurred: {ex.Message}");
            }

            return false;
        }
        private void TestQuery()
        {
            string query = "SELECT totalwaste FROM users WHERE id = 1;";

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            // Assuming totalwaste is an integer, you can cast accordingly
                            int totalWaste = Convert.ToInt32(result);
                            MessageBox.Show($"Total Waste for user with ID 1: {totalWaste}", "Query Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No data found for the given query.", "Query Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Display the error message if an exception occurs
                ShowNotification("Error", $"An error occurred while executing the query: {ex.Message}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TestQuery();
        }
    }
}
