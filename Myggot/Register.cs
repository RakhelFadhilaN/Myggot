using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using Myggot.Classes;
using Npgsql;
using DotNetEnv;

namespace Myggot
{
    public partial class Register : Form
    {
        string connectionString;

        public Register()
        {
            InitializeComponent();
            this.Size = new Size(1920, 1080);
            connectionString = Program.connectionString;

            comboBox1.Items.Add("Pemilik Sampah");
            comboBox1.Items.Add("Petani");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string roles = comboBox1.SelectedItem?.ToString();

            switch (roles)
            {
                case "Pemilik Sampah":
                    textBox6.Enabled = true;
                    break;
                case "Petani":
                    textBox6.Enabled = false;
                    textBox6.Text = ""; // Optionally clear if disabled
                    break;
                default:
                    textBox6.Enabled = false;
                    break;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string selectedRole = comboBox1.SelectedItem?.ToString();

            if (selectedRole == "Pemilik Sampah")
            {
                User user = new User
                {
                    Username = textBox1.Text,
                    Telephone = textBox2.Text,
                    Address = textBox3.Text,
                    Email = textBox4.Text,
                    Password = HashPassword(textBox5.Text)
                };

                string confirmPassword = textBox6.Text;


                if (IsRegistrationValid(user, confirmPassword))
                {
                    RegisterPerson(user, "Pemilik Sampah");
                }
                else
                {
                    ShowNotification("Registrasi gagal", "Pastikan semua data terisi dengan benar.");
                }
            }
            else if (selectedRole == "Petani")
            {
                Farmer farmer = new Farmer
                {
                    Username = textBox1.Text,
                    Telephone = textBox2.Text,
                    Address = textBox3.Text,
                    Email = textBox4.Text,
                    Password = HashPassword(textBox5.Text)
                };
                string confirmPassword = textBox6.Text;

                if (farmer.IsValid())
                {
                    RegisterPerson(farmer, "Petani");
                }
                else
                {
                    ShowNotification("Registrasi gagal", "Pastikan semua data terisi dengan benar.");
                }
            }
        }



        private void RegisterPerson(IRegistrable person, string role)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "";

                    if (role == "Pemilik Sampah")
                    {
                        query = "INSERT INTO users (username, telephone, address, email, password) VALUES (@username, @telephone, @address, @email, @password)";
                    }
                    else if (role == "Petani")
                    {
                        query = "INSERT INTO farmer (username, telephone, address, email, password) VALUES (@username, @telephone, @address, @email, @password)";
                    }

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("username", person.Username);
                        cmd.Parameters.AddWithValue("telephone", person.Telephone);
                        cmd.Parameters.AddWithValue("address", person.Address);
                        cmd.Parameters.AddWithValue("email", person.Email);
                        cmd.Parameters.AddWithValue("password", person.Password); // Hashed password

                        cmd.ExecuteNonQuery();
                    }
                }
                ShowNotification("Registrasi berhasil", $"Akun {role} berhasil dibuat.");
            }
            catch (Exception ex)
            {
                ShowNotification("Error", $"An error occurred: {ex.Message}");
            }
        }



        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Hide();
        }

        private void ShowNotification(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsRegistrationValid(User user, string confirmPassword)
        {
            if (string.IsNullOrEmpty(user.Username) ||
                string.IsNullOrEmpty(user.Telephone) ||
                string.IsNullOrEmpty(user.Address) ||
                string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Password))
            {
                return false; // Ensure all fields are filled
            }

            // Hash the confirm password for comparison
            string hashedConfirmPassword = HashPassword(confirmPassword);

            if (user.Password != hashedConfirmPassword)
            {
                ShowNotification("Password Mismatch", "Password dan konfirmasi password tidak cocok.");
                return false;
            }

            if (!IsValidEmail(user.Email))
            {
                ShowNotification("Email Invalid", "Masukkan alamat email yang benar.");
                return false;
            }

            return true;
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



        

    }
}
