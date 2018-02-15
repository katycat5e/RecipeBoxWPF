using System.Windows;
using System.Windows.Media;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>Main window for the application</summary>
        public static RecipeListWindow RecipeListView;

        private static SplashDialog SplashPage;

        private void Program_Startup(object sender, StartupEventArgs e)
        {
            SplashPage = new SplashDialog();
            SplashPage.Show();

            //if (!EnsureDBExists()) Shutdown();
            
            if (TryFindResource("GlobalUnitManager") is UnitManager unitManager)
            {
                unitManager.UpdateUnitsTable();
            }

            RecipeListView = new RecipeListWindow();
            RecipeListView.ReloadTable(null, null);

            Current.MainWindow = RecipeListView;
            RecipeListView.Show();
            SplashPage.Close();
        }

        private void Test_Startup(object sender, StartupEventArgs e)
        {
            var window = new UnitEditorView();
            window.ShowDialog();
            Current.Shutdown();
        }

        /// <summary>Log a message the console</summary>
        /// <param name="message"></param>
        public static void LogMessage(string message)
        {
            System.Console.WriteLine(message);
        }

        /// <summary>Log an exception to the console</summary>
        /// <param name="e"></param>
        public static void LogException(System.Exception e)
        {
            LogMessage(e.ToString());
        }
    }
}
