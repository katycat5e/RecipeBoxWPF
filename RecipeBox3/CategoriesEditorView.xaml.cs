using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Shapes;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for DBTableEditorView.xaml
    /// </summary>
    public partial class CategoriesView : Window
    {
        private CategoriesAdapter adapter;

        private List<Category> deletedItems = new List<Category>();

        private void SetCategories(IEnumerable<Category> categories)
        {
            var context = DataContext as CategoriesViewModel;
            if (context == null) return;
            else
            {
                if (categories == null) context.Items = null;
                else context.Items = new ObservableCollection<Category>(categories);
            }
        }

        public CategoriesView()
        {
            InitializeComponent();
            
            adapter = new CategoriesAdapter();
            SetupCollection();
        }
        
        public CategoriesView(string connection)
        {
            InitializeComponent();
            
            adapter = new CategoriesAdapter(connection);
            SetupCollection();
        }

        private void SetupCollection()
        {
            SetCategories(adapter.SelectAll());
            if (DataContext is CategoriesViewModel context)
            {
                context.Items.CollectionChanged += Categories_CollectionChanged;
            }
        }

        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // items removed
                foreach (Category row in e.OldItems)
                {
                    if (!deletedItems.Contains(row))
                    {
                        row.Status = RowStatus.Deleted;
                        deletedItems.Add(row);
                    }
                }
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Update DB
            if (DataContext is CategoriesViewModel context)
            {
                adapter.Update(context.Items);
                adapter.Update(deletedItems);
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        
    }

    public class CategoriesViewModel : DependencyObject
    {
        public ObservableCollection<Category> Items
        {
            get { return (ObservableCollection<Category>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<Category>), typeof(CategoriesViewModel), new PropertyMetadata(null));



        public CategoriesViewModel()
        {
        }
    }
}
