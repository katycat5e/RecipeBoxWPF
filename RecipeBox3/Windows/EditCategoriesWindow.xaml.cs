using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;
using System.Windows;
using System.Windows.Controls;

namespace RecipeBox3.Windows
{
    /// <summary>
    /// Interaction logic for DBTableEditorView.xaml
    /// </summary>
    public partial class EditCategoriesWindow : Window
    {
        private CategoriesViewModel ViewModel
        {
            get => DataContext as CategoriesViewModel;
            set => DataContext = value;
        }

        /// <summary>Create a new category editor</summary>
        public EditCategoriesWindow()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Update DB
            ViewModel?.SaveItems();
            if (Application.Current.TryFindResource("GlobalCategoryManager") is CategoryManager categoryManager)
                categoryManager.UpdateTable();

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AddRowButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.AddItem();
        }

        private void DeleteRowButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.DeleteSelectedItem();
        }

        private void CategoriesGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Row.Item is Category item)
            {
                if (!item.IsUserEditable) e.Cancel = true;
            }
        }
    }

    /// <summary>View Model for the categories editor</summary>
    public class CategoriesViewModel : TableEditorViewModel<Category, CategoriesAdapter>
    {
        /// <summary>Create a new instance</summary>
        public CategoriesViewModel()
        {
        }
    }
}
