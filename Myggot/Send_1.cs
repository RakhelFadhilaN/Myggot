using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Myggot.Classes;
using Myggot;
using Npgsql;

namespace MaggotInterface
{
    public partial class Send_1 : Form
    {
        private string connectionString;
        private LoggedInAccount loggedInAccount;
        private Image uploadedImage = null;

        public Send_1(LoggedInAccount loggedInAccount)
        {
            InitializeComponent();
            this.Size = new Size(1920, 1080);
            this.loggedInAccount = loggedInAccount;
            connectionString = Program.connectionString;
        }

        private void Send_Load(object sender, EventArgs e)
        {
            guna2ComboBox1.Items.AddRange(new string[] { "Organik", "Anorganik", "Lainnya" });
            guna2ComboBox2.Items.AddRange(new string[] { "Kurang dari 1 kg", "1-5 kg", "Lebih dari 5 kg" });
            guna2ComboBox3.Items.AddRange(new string[] { "1 hari", "2-3 hari", "Lebih dari 3 hari" });
            guna2ComboBox4.Items.AddRange(new string[] { "Lokasi 1", "Lokasi 2", "Lokasi 3" });
        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton2_Click(object sender, EventArgs e)
        {
            Send_2 change = new Send_2(loggedInAccount);

            change.Show();

            this.Hide();
        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {
            Send_3 change = new Send_3(loggedInAccount);

            change.Show();

            this.Hide();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            // Dispose of the stored image if it exists
            if (uploadedImage != null)
            {
                uploadedImage.Dispose();
            }
            // Dispose of the current image in the ImageButton if it's not the default
            
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Pilih Foto Sampah";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Dispose of previous image if it exists
                        if (uploadedImage != null)
                        {
                            uploadedImage.Dispose();
                        }

                        // Create a new copy of the image from the file
                        using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                        {
                            uploadedImage = Image.FromStream(stream);
                            // Create a new bitmap to avoid file access issues
                            var bitmap = new Bitmap(uploadedImage);

                            // Dispose of current image in ImageButton if it's not the default
                            

                            guna2ImageButton1.Image = bitmap;
                        }

                        MessageBox.Show("Foto berhasil diunggah!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saat mengunggah foto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void guna2Shapes4_Click(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate ComboBox selections
                if (guna2ComboBox1.SelectedItem == null ||
                    guna2ComboBox2.SelectedItem == null ||
                    guna2ComboBox3.SelectedItem == null ||
                    guna2ComboBox4.SelectedItem == null)
                {
                    MessageBox.Show("Harap lengkapi semua data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate logged in account
                if (loggedInAccount == null)
                {
                    MessageBox.Show("Tidak ada akun yang masuk.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int? loggedInId = loggedInAccount.GetLoggedInAccountId();
                if (!loggedInId.HasValue)
                {
                    MessageBox.Show("Tidak dapat menentukan ID akun yang masuk.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string jenisSampah = guna2ComboBox1.SelectedItem.ToString();
                string beratSampah = guna2ComboBox2.SelectedItem.ToString();
                string usiaSampah = guna2ComboBox3.SelectedItem.ToString();
                string lokasi = guna2ComboBox4.SelectedItem.ToString();

                // Validate connection string
                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Connection string tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get address with error handling
                bool isFarmer = loggedInAccount.IsFarmer();
                string orderAddress = GetAddressFromDatabase(loggedInId.Value, isFarmer);

                if (string.IsNullOrEmpty(orderAddress))
                {
                    MessageBox.Show("Alamat tidak ditemukan dalam database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int weight = beratSampah.Contains("1-5") ? 5 : beratSampah.Contains("kurang") ? 1 : 10;
                int points = weight < 1 ? 10 : weight == 5 ? 50 : 100;

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = @"
                    INSERT INTO orders (orderaddress, orderdate, orderuserid, weight, point, initializeorder)
                    VALUES (@address, @orderdate, @userid, @weight, @points, false)";

                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("address", orderAddress);
                            cmd.Parameters.AddWithValue("orderdate", DateTime.Now);
                            cmd.Parameters.AddWithValue("userid", loggedInId.Value);
                            cmd.Parameters.AddWithValue("weight", weight);
                            cmd.Parameters.AddWithValue("points", points);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Data berhasil disimpan, penjemputan akan diproses, dan poin telah ditambahkan.",
                                    "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Gagal menyimpan data ke database.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        MessageBox.Show($"Database error: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi kesalahan: {ex.Message}\n\nStack Trace: {ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        public string GetAddressFromDatabase(int accountId, bool isFarmer)
        {
            string address = string.Empty;
            string tableName = isFarmer ? "farmer" : "users";

            using (var conn = new NpgsqlConnection(connectionString))  // Use the class-level connectionString
            {
                conn.Open();
                string query = $"SELECT address FROM {tableName} WHERE id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("id", accountId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            address = reader["address"].ToString();
                        }
                    }
                }
            }

            return address;
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
    }
}
