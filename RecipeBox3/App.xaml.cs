using System.Windows;
using System.Windows.Media;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
            var cid = new ViewRecipeWindow(2);
            cid.ShowDialog();
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static void LogMessage(string message)
        {
            System.Console.WriteLine(message);
        }

        public static void LogException(System.Exception e)
        {
            LogMessage(e.ToString());
        }
    }
}
