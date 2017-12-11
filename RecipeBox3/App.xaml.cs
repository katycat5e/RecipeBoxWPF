using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static CookbookAdapter Adapter;
        public static MainWindow RecipeListView;
        public static UnitManager UnitManager;

        private static SplashDialog SplashPage;

        private void Program_Startup(object sender, StartupEventArgs e)
        {
            SplashPage = new SplashDialog();
            SplashPage.Show();

            if (!EnsureDBExists()) Shutdown();
            
            Adapter = new CookbookAdapter();
            UnitManager = new UnitManager();

            RecipeListView = new MainWindow();
            RecipeListView.ReloadTable(null, null);

            Current.MainWindow = RecipeListView;
            RecipeListView.Show();
            SplashPage.Close();
        }
    }
}
