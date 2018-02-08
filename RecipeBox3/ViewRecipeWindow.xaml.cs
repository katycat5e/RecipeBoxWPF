using System;
using System.Collections.Generic;
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
    /// Interaction logic for ViewRecipeWindow.xaml
    /// </summary>
    public partial class ViewRecipeWindow : Window
    {
        public ViewRecipeWindow()
        {
            InitializeComponent();

            if (TryFindResource("UnitStringConverter") is AmountStringConverter unitStringConverter)
            {
                unitStringConverter.UnitManager?.UpdateUnitsTable();
            }
        }

        public ViewRecipeWindow(int recipeID) : this()
        {
            if (DataContext is ViewRecipeViewModel viewModel)
            {
                viewModel.RecipeID = recipeID;
            }
        }
    }
}
