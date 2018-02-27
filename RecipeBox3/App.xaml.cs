using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged
    {
        /// <summary>Main window for the application</summary>
        public static RecipeListWindow RecipeListView;

        private static SplashDialog SplashPage;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private Unit.System _UnitSystem = Unit.System.Any;
        /// <summary>Setting for which Unit system to be displayed</summary>
        public Unit.System UnitSystem
        {
            get => _UnitSystem;
            set
            {
                if (value != _UnitSystem)
                {
                    _UnitSystem = value;
                    RecipeBox3.Properties.Settings.Default.SelectedUnitSystem = value.GetString();
                    OnPropertyChanged("UnitSystem");
                }
            }
        }

        private bool _ShowPreviewImages = true;
        /// <summary>Setting of whether to show preview images in recipe list</summary>
        public bool ShowPreviewImages
        {
            get => _ShowPreviewImages;
            set
            {
                if (value != _ShowPreviewImages)
                {
                    _ShowPreviewImages = value;
                    RecipeBox3.Properties.Settings.Default.ShowPreviewImages = value;
                    OnPropertyChanged("ShowPreviewImages");
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Program_Startup(object sender, StartupEventArgs e)
        {
            SplashPage = new SplashDialog();
            SplashPage.Show();

            var dbChecker = new DBChecker(RecipeBox3.Properties.Settings.Default.SQLiteConnectionString);
            // TODO fix this binding
            var progressBinding = new Binding
            {
                Source = dbChecker,
                Path = new PropertyPath(DBChecker.OperationProgressProperty),
                Mode = BindingMode.OneWay
            };

            

            if (!dbChecker.CheckDatabase())
            {
                Shutdown();
                return;
            }

            if (Enum.TryParse(RecipeBox3.Properties.Settings.Default.SelectedUnitSystem, out Unit.System system))
                UnitSystem = system;

            // Update table caches
            if (TryFindResource("GlobalUnitManager") is UnitManager unitManager)
                unitManager.UpdateTable();

            if (TryFindResource("GlobalCategoryManager") is CategoryManager categoryManager)
                categoryManager.UpdateTable();

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
            Console.WriteLine(message);
        }

        /// <summary>Log an exception to the console</summary>
        /// <param name="e"></param>
        public static void LogException(Exception e)
        {
            LogMessage(e.ToString());
        }
    }
}
