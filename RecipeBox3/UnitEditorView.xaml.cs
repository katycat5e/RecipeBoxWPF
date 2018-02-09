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
    public partial class UnitEditorView : Window
    {
        private UnitsAdapter adapter;

        private UnitEditorViewModel ViewModel
        {
            get => DataContext as UnitEditorViewModel;
            set => DataContext = value;
        }

        public static Dictionary<Unit.UnitType, string> UnitTypeDict =
            Enum.GetValues(typeof(Unit.UnitType))
            .Cast<Unit.UnitType>()
            .ToDictionary(p => p, p => p.GetString());

        public static Dictionary<Unit.System, string> UnitSystemDict =
            Enum.GetValues(typeof(Unit.System))
            .Cast<Unit.System>()
            .ToDictionary(p => p, p => p.GetString());

        private List<Unit> deletedItems = new List<Unit>();

        private void SetUnits(IEnumerable<Unit> units)
        {
            if (ViewModel == null) return;
            else
            {
                if (units == null) ViewModel.Items = null;
                else ViewModel.Items = new ObservableCollection<Unit>(units);
            }
        }

        public UnitEditorView()
        {
            InitializeComponent();
            
            adapter = new UnitsAdapter();
            SetupCollection();
        }

        private void SetupCollection()
        {
            SetUnits(adapter.SelectAll());
            if (ViewModel != null)
            {
                ViewModel.Items.CollectionChanged += Units_CollectionChanged;
            }
        }

        private void Units_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // items removed
                foreach (Unit row in e.OldItems)
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
            if (ViewModel != null)
            {
                adapter.Update(ViewModel.Items);
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

        // prevent user from editing read only rows
        private void UnitsGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Row.Item is Unit selectedUnit)
            {
                if (!selectedUnit.IsUserEditable)
                {
                    e.Cancel = true;
                }
            }
        }

        private void AddRowButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.AddUnit();
        }

        private void DeleteRowButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.DeleteSelectedUnit();
        }
    }

    public class UnitEditorViewModel : DependencyObject
    {
        public ObservableCollection<Unit> Items
        {
            get { return (ObservableCollection<Unit>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<Unit>), typeof(UnitEditorViewModel), new PropertyMetadata(null));



        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(UnitEditorViewModel), new PropertyMetadata(null));



        public UnitEditorViewModel()
        {
            
        }

        public void AddUnit()
        {
            Items.Add(new Unit());
        }

        public void DeleteSelectedUnit()
        {
            if (SelectedItem is Unit selectedUnit)
            {
                if (selectedUnit.IsUserEditable) Items.Remove(selectedUnit);
            }
        }
    }
}
