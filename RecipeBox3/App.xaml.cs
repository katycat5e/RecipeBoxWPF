using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static CookbookAdapter Adapter;
        public static RecipeListWindow RecipeListView;
        public static UnitManager UnitManager;

        private static SplashDialog SplashPage;

        private void Program_Startup(object sender, StartupEventArgs e)
        {
            SplashPage = new SplashDialog();
            SplashPage.Show();

            if (!EnsureDBExists()) Shutdown();
            
            Adapter = new CookbookAdapter();
            UnitManager = new UnitManager();

            RecipeListView = new RecipeListWindow();
            RecipeListView.ReloadTable(null, null);

            Current.MainWindow = RecipeListView;
            RecipeListView.Show();
            SplashPage.Close();
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
    }
}
