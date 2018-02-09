using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for DBTableEditorView.xaml
    /// </summary>
    public partial class UnitEditorView : Window
    {
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

        public UnitEditorView()
        {
            InitializeComponent();
        }
        

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Update DB
            ViewModel?.SaveItems();
            if (Application.Current.TryFindResource("GlobalUnitManager") is UnitManager unitManager)
            {
                unitManager.UpdateUnitsTable();
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
            ViewModel?.AddItem();
        }

        private void DeleteRowButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.DeleteSelectedItem();
        }
    }

    public class UnitEditorViewModel : TableEditorViewModel<Unit, UnitsAdapter>
    {
        public UnitEditorViewModel() : base()
        {

        }
    }
}
