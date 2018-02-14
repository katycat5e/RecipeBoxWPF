using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace RecipeBox3
{
    partial class App
    {
        /// <summary>
        /// Check that the database is setup correctly
        /// </summary>
        /// <returns></returns>
        public static bool EnsureDBExists()
        {
            bool successful = false;

            SplashPage.StatusText = "Checking database...";

            using (var tmpClient = new SqlConnection(GetInitialConnectionString((string)RecipeBox3.Properties.Settings.Default["CookbookConnectionString"])))
            {
                try
                {
                    tmpClient.Open();

                    var tmpCommand = new SqlCommand() { Connection = tmpClient };
                    tmpCommand.CommandText = "SELECT name FROM sys.databases WHERE name='Cookbook'";

                    bool schemaExists = false;
                    using (var reader = tmpCommand.ExecuteReader())
                    {
                        if (reader.HasRows) schemaExists = true;
                    }

                    if (!schemaExists) successful = InitializeDB(tmpClient);
                    else successful = true;
                }
                catch (SqlException)
                {
                    //MessageBox.Show(DB_CONNECT_ERROR + e.Message, "Error Connecting to Database",
                    //    MessageBoxButtons.OK, MessageBoxIcon.Error);

                    successful = InstallDB();

                    if (successful)
                    {
                        try
                        {
                            tmpClient.Open();

                            var tmpCommand = new SqlCommand() { Connection = tmpClient };
                            tmpCommand.CommandText = "SELECT name FROM sys.databases WHERE name='Cookbook'";

                            using (var reader = tmpCommand.ExecuteReader())
                            {
                                if (!reader.HasRows) InitializeDB(tmpClient);
                            }

                            successful = true;
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show(DB_CONNECT_ERROR + ex.Message, "Error Connecting to Database",
                                MessageBoxButton.OK, MessageBoxImage.Error);

                            successful = false;
                        }
                    }
                }
            }

            return successful;
        }

        private static bool InstallDB()
        {
            string resourcesPath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\";
            string localDBInstallCommand = @"C:\Windows\System32\msiexec";
            string DBInstallArgs = @"/i """ + resourcesPath + @"\SQLLocalDB.msi"" /qn IACCEPTSQLLOCALDBLICENSETERMS=YES";
            //string localDBInstallCommand = resourcesPath + "installDB.bat";

            if (!File.Exists("C:\\Program Files\\Microsoft SQL Server\\120\\Tools\\Binn\\SqlLocalDB.exe"))
            {
                var result = MessageBox.Show(
                "SQL Server LocalDB not found, it must be installed to continue. Install Microsoft SQL Server LocalDB?",
                "Install LocalDB", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

                if (result != MessageBoxResult.Yes) return false;

                SplashPage.StatusText = "Installing database driver...";

                try
                {
                    using (var installProcess = new Process())
                    {
                        installProcess.StartInfo = new ProcessStartInfo(localDBInstallCommand, DBInstallArgs)//, resourcesPath + "\\SqlLocalDB.msi")
                        {
                            Verb = "runas"
                        };

                        installProcess.Start();
                        installProcess.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL Server LocalDB did not install correctly, application will now exit.\n\n" + ex.Message,
                        "LocalDB failed to install", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
                if (!File.Exists("C:\\Program Files\\Microsoft SQL Server\\120\\Tools\\Binn\\SqlLocalDB.exe"))
                {
                    MessageBox.Show("SQL Server LocalDB did not install correctly, application will now exit.",
                        "LocalDB failed to install", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            // setup RecipeDB instance
            try
            {
                using (var dbCreateProcess = new Process())
                {
                    dbCreateProcess.StartInfo = new ProcessStartInfo("C:\\Program Files\\Microsoft SQL Server\\120\\Tools\\Binn\\SqlLocalDB.exe", "create RecipeBox 12.0");
                    dbCreateProcess.Start();
                    dbCreateProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create the RecipeBox database instance, application will now exit.\n\n" + ex.Message,
                        "Could not create instance", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private static bool InitializeDB(SqlConnection client)
        {
            bool successful = false;

            SplashPage.StatusText = "Creating cookbook schema...";

            var tmpCommand = new SqlCommand("", client);

            try
            {
                string dbPath = "C:\\RecipeBox\\";
                if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);

                var commandLines = Regex.Split(RecipeBox3.Properties.Resources.DBCreateScript, "GO\r\n", RegexOptions.Multiline);
                foreach (string nextCommand in commandLines)
                {
                    if (string.IsNullOrWhiteSpace(nextCommand)) continue;
                    tmpCommand.CommandText = nextCommand;
                    tmpCommand.ExecuteNonQuery();
                }

                successful = true;
            }
            catch (Exception e)
            {
                successful = false;
                MessageBox.Show(DB_CREATE_ERROR + e.Message,
                        "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return successful;
        }

        private static string GetInitialConnectionString(string connectionString)
        {
            int start = connectionString.IndexOf("Initial Catalog=");
            int end = connectionString.IndexOf(';', start);

            if (start < 0 || end < 0) return connectionString;
            else
            {
                return connectionString.Substring(0, start) + connectionString.Substring(end + 1, connectionString.Length - end - 1);
            }
        }

        private const string DB_CONNECT_ERROR =
            "A connection to the database could not be opened.\n\nError message:\n\n";

        private const string DB_CREATE_ERROR =
            "Database connection succeeded, but the cookbook schema could not be created\n\nError message:\n\n";
    }
}
