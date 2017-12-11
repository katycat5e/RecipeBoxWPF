using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CookbookDataSet DataSet { get; set; }
        private CookbookAdapter Adapter { get { return App.Adapter; } }

        public MainWindow()
        {
            InitializeComponent();
            DataSet = new CookbookDataSet();
            DataContext = this;
        }

        public void ReloadTable(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            try
            {
                Adapter.SimpleRecipeViewTableAdapter.Fill(DataSet.SimpleRecipeView);
            }
            catch (SqlException ex)
            {
                DataSet.SimpleRecipeView.Clear();

                MessageBox.Show("An error occurred while downloading the data:\n\n" + ex.Message,
                    "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Cursor = Cursors.Arrow;
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var settingsDialog = new EditSettingsDialog { Owner = this };
            settingsDialog.ShowDialog();
        }

        private void RecipeListView_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
