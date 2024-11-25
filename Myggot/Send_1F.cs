using MaggotInterface;
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

namespace Myggot
{
    public partial class Send_1F : Form
    {
        private LoggedInAccount loggedInAccount;
        private string connectionString;
        public Send_1F(LoggedInAccount loggedInAccount)
        {
            InitializeComponent();
            this.Size = new Size(1920, 1080);
            this.loggedInAccount = loggedInAccount;

            // Debug connection string
            if (string.IsNullOrEmpty(Program.connectionString))
            {
                MessageBox.Show("Connection string is null or empty", "Error");
                return;
            }
            connectionString = Program.connectionString;
            Console.WriteLine($"Connection string: {connectionString}");

            InitializeListView();
            PopulateOrderListView();
            listViewRecent.SelectedIndexChanged += listViewRecent_SelectedIndexChanged;
        }


        private void label6_Click(object sender, EventArgs e)
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

        private void IconChat_Click(object sender, EventArgs e)
        {

        }



        private void InitializeListView()
        {
            listViewRecent.View = View.Details;
            listViewRecent.FullRowSelect = true;
            listViewRecent.GridLines = true; // Added for better visibility

            // Match the exact order we'll display the data
            listViewRecent.Columns.Add("Order ID",0);  // Made visible for debugging
            listViewRecent.Columns.Add("Date", 150);
            listViewRecent.Columns.Add("Weight", 100);
            listViewRecent.Columns.Add("Address", 800);
       
        }

        private void PopulateOrderListView()
        {
            listViewRecent.Items.Clear();

            string query = @"
            SELECT orderid, orderaddress, orderdate, weight, initializeorder, endorder
            FROM orders 
            WHERE initializeorder = FALSE 
            AND endorder = FALSE
            ORDER BY orderdate DESC;";  // Added ORDER BY for newest first

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    // First, let's debug what's in the database
                    using (var debugCmd = new NpgsqlCommand("SELECT COUNT(*) FROM orders", connection))
                    {
                        int totalOrders = Convert.ToInt32(debugCmd.ExecuteScalar());
                        System.Diagnostics.Debug.WriteLine($"Total orders in database: {totalOrders}");
                    }

                    using (var command = new NpgsqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        bool hasRows = false;
                        while (reader.Read())
                        {
                            hasRows = true;
                            var item = new ListViewItem(reader["orderid"].ToString());
                            item.SubItems.Add(((DateTime)reader["orderdate"]).ToString("yyyy-MM-dd"));
                            item.SubItems.Add(reader["weight"].ToString() ?? "N/A");
                            item.SubItems.Add(reader["orderaddress"].ToString());

                            listViewRecent.Items.Add(item);

                            // Debug output
                            System.Diagnostics.Debug.WriteLine($"Added order: {reader["orderid"]}, Initialize: {reader["initializeorder"]}, End: {reader["endorder"]}");
                        }

                        if (!hasRows)
                        {
                            // Debug query to see why no rows match
                            string debugQuery = @"
                            SELECT 
                                COUNT(*) as total,
                                SUM(CASE WHEN initializeorder = TRUE THEN 1 ELSE 0 END) as initialized,
                                SUM(CASE WHEN endorder = FALSE THEN 1 ELSE 0 END) as not_ended
                            FROM orders;";

                            connection.Close();
                            connection.Open();
                            using (var debugCmd = new NpgsqlCommand(debugQuery, connection))
                            using (var debugReader = debugCmd.ExecuteReader())
                            {
                                if (debugReader.Read())
                                {
                                    MessageBox.Show(
                                        $"No matching orders found.\n" +
                                        $"Total orders: {debugReader["total"]}\n" +
                                        $"Initialized orders: {debugReader["initialized"]}\n" +
                                        $"Not ended orders: {debugReader["not_ended"]}",
                                        "Debug Info",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}\n\nStack Trace: {ex.StackTrace}",
                              "Database Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }


        private void BtnTakeOrder_Click(object sender, EventArgs e)
        {
            if (listViewRecent.SelectedItems.Count > 0)
            {
                var selectedItem = listViewRecent.SelectedItems[0];
                int orderId = int.Parse(selectedItem.Text);

                string updateQuery = @"
            UPDATE orders
            SET orderfarmerid = @farmerId, initializeorder = FALSE
            WHERE orderid = @orderId;";

                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        using (var command = new NpgsqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@farmerId", loggedInAccount.GetLoggedInAccountId());
                            command.Parameters.AddWithValue("@orderId", orderId);
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Order taken successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateOrderListView(); // Refresh the list
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error taking the order: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an order to take.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void BtnFinishOrder_Click(object sender, EventArgs e)
        {
            if (listViewRecent.SelectedItems.Count > 0)
            {
                var selectedItem = listViewRecent.SelectedItems[0];
                int orderId = int.Parse(selectedItem.Text);

                string updateQuery = @"
            UPDATE orders
            SET endorder = TRUE
            WHERE orderid = @orderId;";

                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        using (var command = new NpgsqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@orderId", orderId);
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Order finished successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateOrderListView(); // Refresh the list
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error finishing the order: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an order to finish.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void listViewRecent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewRecent.SelectedItems.Count > 0)
            {
                var selectedItem = listViewRecent.SelectedItems[0];

                // Match the new column order
                string orderId = selectedItem.SubItems[0].Text;
                string date = selectedItem.SubItems[1].Text;
                string address = selectedItem.SubItems[2].Text;
                string weight = selectedItem.SubItems[3].Text;

                // Display the selected order details
                MessageBox.Show(
                    $"Selected Order:\n" +
                    $"Order ID: {orderId}\n" +
                    $"Date: {date}\n" +
                    $"Address: {address}\n" +
                    $"Weight: {weight}",
                    "Order Details",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }


    }
}
