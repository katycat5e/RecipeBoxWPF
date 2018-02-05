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
    /// Interaction logic for EditRecipeDialog.xaml
    /// </summary>
    public partial class EditRecipeDialog : Window
    {
        public EditRecipeDialog()
        {
            InitializeComponent();
        }

        public EditRecipeDialog(int recipeID)
        {
            if (DataContext is ViewRecipeViewModel viewModel)
            {
                viewModel.RecipeID = recipeID;
            }
        }

        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
