using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for RecipeListWindow.xaml
    /// </summary>
    public partial class RecipeListWindow : Window
    {
        private RecipeListViewModel ViewModel => DataContext as RecipeListViewModel;

        /// <summary>Create a new instance of the class</summary>
        public RecipeListWindow()
        {
            InitializeComponent();
            ShowImagesMenuItem.IsChecked = Properties.Settings.Default.ShowPreviewImages;
        }

        /// <summary>Refresh the list of recipes</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReloadTable(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            if (ViewModel != null)
            {
                ViewModel.GetAllRecipes();
                if (ViewModel.ShowImages) ViewModel.UpdateImages();
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


        // Event Handlers
        //--------------------------------------------------------------------------------------------------------

        private void ImgReload_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.UpdateImages();
        }

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static void ShowRecipeDetails(int recipeID)
        {
            var viewRecipeWindow = new ViewRecipeWindow(recipeID);

            viewRecipeWindow.Show();
            viewRecipeWindow.Focus();
        }

        private void CreateNewRecipe()
        {
            var recipeEditor = new EditRecipeDialog();
            bool? result = recipeEditor.ShowDialog();
            if (result == true) ReloadTable(this, new RoutedEventArgs());
        }

        private void OpenRecipeForEdit(int recipeID)
        {
            var recipeEditor = new EditRecipeDialog(recipeID);
            bool? result = recipeEditor.ShowDialog();
            if (result == true) ReloadTable(this, new RoutedEventArgs());
        }
        
        private void RecipeGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel?.SelectedGridItem is DetailRecipe row)
            {
                ShowRecipeDetails(row.R_ID);
            }
        }

        private void RecipeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem item)) return;

            if (item.Name == "NewRecipeMenuItem")
            {
                CreateNewRecipe();
                return;
            }

            // Recipe Options
            if (ViewModel?.SelectedGridItem is DetailRecipe selectedRow)
            {
                switch (item.Name)
                {
                    case "ViewRecipeMenuItem":
                    case "ViewRecipeContextItem":
                        ShowRecipeDetails(selectedRow.ID);
                        return;

                    case "EditRecipeMenuItem":
                    case "EditRecipeContextItem":
                        OpenRecipeForEdit(selectedRow.ID);
                        return;

                    case "DeleteRecipeMenuItem":
                    case "DeleteRecipeContextItem":
                        if (ViewModel != null)
                        {
                            ViewModel.DeleteRecipe(selectedRow.ID);
                            ReloadTable(sender, e);
                        }
                        return;

                    default:
                        return;
                }
            }
        }

        private void EditCategoriesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var categoryEditor = new CategoriesEditorView
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            categoryEditor.ShowDialog();
        }

        private void EditUnitsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var unitEditor = new UnitEditorView
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            unitEditor.ShowDialog();
        }

        private void ShowImagesMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (IMG_Preview != null) IMG_Preview.Visibility = Visibility.Visible;
        }

        private void ShowImagesMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IMG_Preview != null) IMG_Preview.Visibility = Visibility.Collapsed;
        }

        private void SubmitSearchButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.FilterRecipes();
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.ClearSearch();
        }
    }
}
