using MaggotInterface;
using System;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Net;
using System.Configuration;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace Myggot
{
    internal static class Program
    {
        public static string connectionString;

        public static IConfiguration Configuration { get; private set; }

        static void Main()
        {
            try
            {
                // Set the SSL root certificate path
                Environment.SetEnvironmentVariable("PGSSLROOTCERT", @"C:\Users\Rakhel\AppData\Roaming\postgresql\ca.pem");

                // Build the configuration from appsettings.json
                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                Configuration = configBuilder.Build();

                // Retrieve the connection string
                connectionString = Configuration.GetConnectionString("Database");

                // Add KeepAlive setting when building the connection string
                var npgsqlBuilder = new NpgsqlConnectionStringBuilder(connectionString)
                {
                    SslMode = SslMode.Require,
                    TrustServerCertificate = true, // Temporarily set to true to bypass validation
                    KeepAlive = 10 // Set KeepAlive to 10 seconds
                };

                // Test database connection
                using (var conn = new NpgsqlConnection(npgsqlBuilder.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    {
                        cmd.CommandTimeout = 300; // Set the timeout to 300 seconds
                        cmd.ExecuteScalar(); // Execute a simple test query
                    }
                }

                // Run the Windows Forms application
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Welcome());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
