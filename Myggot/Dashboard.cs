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
using Npgsql;
using Myggot;

namespace MaggotInterface
{
    public partial class Dashboard : Form
    {
        private string connectionString;

        private LoggedInAccount loggedInAccount;
        public Dashboard(LoggedInAccount loggedInAccount)
        {
            InitializeComponent();
            this.Size = new Size(1920, 1080);
            connectionString = Program.connectionString;
            this.loggedInAccount = loggedInAccount;
            label2.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            InitializeListView();

            // Show loading indicator
            using (var loadingForm = new Form())
            {
                loadingForm.StartPosition = FormStartPosition.CenterScreen;
                loadingForm.FormBorderStyle = FormBorderStyle.None;
                loadingForm.Size = new Size(200, 70);
                loadingForm.BackColor = Color.White;

                var label = new Label
                {
                    Text = "Loading dashboard...",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };

                loadingForm.Controls.Add(label);
                loadingForm.Show();

                // Load data asynchronously
                Task.Run(async () =>
                {
                    await LoadDashboardDataAsync();
                    await LoadMonthlyWasteDataAsync();

                    this.Invoke((MethodInvoker)delegate
                    {
                        loadingForm.Close();
                    });
                });
            }
        }

        private void InitializeListView()
        {
            listViewRecent.View = View.Details;
            listViewRecent.FullRowSelect = true;
            listViewRecent.GridLines = true; // Added for better visibility

            // Match the exact order we'll display the data
            listViewRecent.Columns.Add("Order ID", 0);
            listViewRecent.Columns.Add("Order Address", 0);
            listViewRecent.Columns.Add("Order Date", 150);
            listViewRecent.Columns.Add("Weight", 100);


        }

        // Form Load event to display TotalWaste automatically



        // Method to fetch TotalWaste from the database based on the logged-in account
        private string GetTotalWasteFromDatabase()
        {
            if (loggedInAccount == null || !loggedInAccount.IsLoggedIn)
            {
                throw new Exception("User is not logged in.");
            }

            int? accountId = loggedInAccount.GetLoggedInAccountId();
            if (!accountId.HasValue)
            {
                throw new Exception("Logged-in account ID not found.");
            }

            string tableName = loggedInAccount.IsFarmer() ? "farmer" : "users";
            // Modified query to get specific user's total waste
            string query = $"SELECT COALESCE(totalwaste, 0) FROM {tableName} WHERE id = @id;";

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", accountId.Value);

                        object result = command.ExecuteScalar();
                        return result?.ToString() ?? "0";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "0";
            }
        }

        private int GetWeeklyTotalWaste()
        {
            if (loggedInAccount == null || !loggedInAccount.IsLoggedIn)
            {
                throw new Exception("User is not logged in.");
            }

            int accountId = loggedInAccount.GetLoggedInAccountId().GetValueOrDefault();
            bool isFarmer = loggedInAccount.IsFarmer();

            // Modified query to properly handle both user and farmer cases
            string query = @"
        SELECT COALESCE(SUM(o.weight), 0) as weekly_total
        FROM orders o
        WHERE o.endorder = true 
        AND o.orderdate >= CURRENT_DATE - INTERVAL '7 days'
        AND (
            CASE 
                WHEN @isFarmer = false THEN o.orderuserid = @accountId
                ELSE o.orderfarmerid = @accountId
            END
        );";

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@accountId", accountId);
                        command.Parameters.AddWithValue("@isFarmer", isFarmer);

                        object result = command.ExecuteScalar();
                        return Convert.ToInt32(result ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching weekly total: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }


        private void UpdateWeeklyTotalWaste()
        {
            try
            {
                int weeklyTotal = GetWeeklyTotalWaste();
                label7.Text = $"Total Waste This Week: {weeklyTotal} kg";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating weekly total waste: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void alamat_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Shapes2_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void guna2Shapes2_Click_1(object sender, EventArgs e)
        {

        }

        private void Rewards_Load(object sender, EventArgs e)
        {

        }

        private void guna2Shapes3_Click(object sender, EventArgs e)
        {

        }
        

        private void label7_Click(object sender, EventArgs e)
        {

        
        }

        

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void IconDashboard_Click(object sender, EventArgs e)
        {
            Dashboard DashboardForm = new Dashboard(loggedInAccount);
            DashboardForm.Show();
            this.Hide();
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
        public async void UpdateTotalWaste()
        {
            try
            {
                string totalWaste = await Task.Run(() => GetTotalWasteFromDatabase());

                // Update UI on the main thread
                if (this.IsDisposed) return;

                this.Invoke((MethodInvoker)delegate
                {
                    label4.Text = $"Total Waste: {totalWaste} kg";
                });
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error updating total waste: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        label4.Text = "Total Waste: 0 kg";
                    });
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateTotalWaste();
            SetupChart();
            UpdateWeeklyTotalWaste(); // Add this line to update weekly waste on form load
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }


        private async Task LoadDashboardDataAsync()
        {
            if (loggedInAccount == null || !loggedInAccount.IsLoggedIn)
                return;

            try
            {
                // Update the date label using proper Invoke
                this.Invoke((MethodInvoker)delegate
                {
                    label2.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");
                });

                int accountId = loggedInAccount.GetLoggedInAccountId().GetValueOrDefault();
                bool isFarmer = loggedInAccount.IsFarmer();
                string tableName = isFarmer ? "farmer" : "users";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Modified SQL query
                    string query = @"
            WITH user_total AS (
                SELECT totalwaste 
                FROM " + tableName + @" 
                WHERE id = @accountId
            ),
            weekly_total AS (
                SELECT COALESCE(SUM(o.weight), 0) as weekly_total
                FROM orders o
                WHERE o.endorder = true 
                AND o.orderdate >= CURRENT_DATE - INTERVAL '7 days'
                AND CASE 
                    WHEN @isFarmer = false THEN o.orderuserid = @accountId
                    ELSE o.orderfarmerid = @accountId
                END
            ),
            recent_orders AS (
                SELECT orderid, orderaddress, orderdate, weight
                FROM orders
                WHERE endorder = true
                AND CASE 
                    WHEN @isFarmer = false THEN orderuserid = @accountId
                    ELSE orderfarmerid = @accountId
                END
                ORDER BY orderdate DESC
                LIMIT 10
            )
            SELECT 
                (SELECT totalwaste FROM user_total) as total_waste,
                (SELECT weekly_total FROM weekly_total) as weekly_total,
                (SELECT json_agg(row_to_json(recent_orders)) FROM recent_orders) as recent_orders;";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@accountId", accountId);
                        cmd.Parameters.AddWithValue("@isFarmer", isFarmer);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Parse total waste and weekly waste
                                var totalWaste = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                var weeklyWaste = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);

                                // Parse recent orders JSON
                                var recentOrdersJson = reader.IsDBNull(2) ? "[]" : reader.GetString(2);
                                var recentOrders = System.Text.Json.JsonSerializer.Deserialize<List<Order>>(recentOrdersJson);

                                // Update UI labels for total and weekly waste
                                this.Invoke((MethodInvoker)delegate
                                {
                                    label7.Text = $"Total Waste This Week: {weeklyWaste} kg";
                                });

                                // Update ListView with recent orders
                                this.Invoke((MethodInvoker)delegate
                                {
                                    PopulateListView(recentOrders);
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle errors on the UI thread
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    label7.Text = "Total Waste This Week: 0 kg";
                });
            }
        }



        private void PopulateListView(List<Order> orders)
        {
            listViewRecent.Items.Clear(); // Clear existing items

            foreach (var order in orders)
            {
                var item = new ListViewItem(order.OrderId.ToString());
                item.SubItems.Add(order.OrderAddress ?? "N/A");
                item.SubItems.Add(order.OrderDate.ToString("yyyy-MM-dd HH:mm"));
                item.SubItems.Add(order.Weight.HasValue ? $"{order.Weight} kg" : "N/A");

                listViewRecent.Items.Add(item);
            }
        }




        private async Task LoadMonthlyWasteDataAsync()
        {
            if (loggedInAccount == null || !loggedInAccount.IsLoggedIn)
                return;

            try
            {
                int accountId = loggedInAccount.GetLoggedInAccountId().GetValueOrDefault();
                bool isFarmer = loggedInAccount.IsFarmer();

                string query = @"
            SELECT 
                TO_CHAR(o.orderdate, 'YYYY-MM') as month,
                COALESCE(SUM(o.weight), 0) as total_weight
            FROM orders o
            WHERE o.endorder = true
              AND (
                CASE 
                    WHEN @isFarmer = false THEN o.orderuserid = @accountId
                    ELSE o.orderfarmerid = @accountId
                END
              )
            GROUP BY TO_CHAR(o.orderdate, 'YYYY-MM')
            ORDER BY month DESC;
        ";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@accountId", accountId);
                        cmd.Parameters.AddWithValue("@isFarmer", isFarmer);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            // Collect data points in a temporary list
                            var dataPoints = new List<KeyValuePair<string, int>>();

                            while (await reader.ReadAsync())
                            {
                                string month = reader.GetString(0); // Month as 'YYYY-MM'
                                int totalWeight = reader.GetInt32(1); // Total weight for that month
                                dataPoints.Add(new KeyValuePair<string, int>(month, totalWeight));
                            }

                            // Update the chart on the main UI thread
                            this.Invoke((MethodInvoker)delegate
                            {
                                var series = chart1.Series[0];
                                series.Points.Clear(); // Clear existing data

                                foreach (var dataPoint in dataPoints)
                                {
                                    series.Points.AddXY(dataPoint.Key, dataPoint.Value);
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Error loading monthly waste data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }





        private void SetupChart()
        {
            chart1.Series.Clear(); // Clear any existing series
            var series = new System.Windows.Forms.DataVisualization.Charting.Series("MonthlyWaste")
            {
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column,
                XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String,
                YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32
            };
            chart1.Series.Add(series);

            chart1.ChartAreas[0].AxisX.Title = "Month";
            chart1.ChartAreas[0].AxisY.Title = "Total Waste (kg)";
        }


        // Helper classes for JSON deserialization
        private class OrderData
        {
            public DateTime orderdate { get; set; }
            public int weight { get; set; }
        }

        private class WasteHistoryData
        {
            public string month { get; set; }
            public int total_waste { get; set; }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void listViewRecent_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
