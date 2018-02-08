using System;
using System.Collections.Generic;
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
    /// Interaction logic for EditRecipeDialog.xaml
    /// </summary>
    public partial class EditRecipeDialog : Window
    {
        private EditRecipeViewModel ViewModel
        {
            get => DataContext as EditRecipeViewModel;
            set => DataContext = value;
        }

        public EditRecipeDialog()
        {
            InitializeComponent();

            if (ViewModel != null)
            {
                ViewModel.MyRecipe = new SQLiteModel.Data.DetailRecipe();
                ViewModel.UnitManager?.UpdateUnitsTable();
            }
        }

        public EditRecipeDialog(int? recipeID)
        {
            InitializeComponent();

            if (ViewModel != null && recipeID.HasValue)
            {
                ViewModel.RecipeID = recipeID;
                ViewModel.UnitManager?.UpdateUnitsTable();
            }
        }

        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var imagePicker = new ChooseImageDialog();
            Cursor = Cursors.Wait;
            bool? result = imagePicker.ShowDialog();

            if (result == true && ViewModel != null)
            {
                ViewModel.MyRecipe.IMG_Data = ByteImageConverter.ConvertBitmapToBytes(imagePicker.FinalBitmap);
            }
            Cursor = Cursors.Arrow;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = ViewModel?.SaveRecipe();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        
    }
}
