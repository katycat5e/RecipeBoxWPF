using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for ViewRecipeWindow.xaml
    /// </summary>
    public partial class ViewRecipeWindow : Window
    {
        /// <summary>Create a new blank viewer</summary>
        public ViewRecipeWindow()
        {
            InitializeComponent();
            
            ((App)Application.Current).PropertyChanged += App_PropertyChanged;
        }

        /// <summary>Create a new viewer for the specified recipe</summary>
        /// <param name="recipeID">The ID of the desired recipe to view</param>
        public ViewRecipeWindow(int recipeID) : this()
        {
            if (DataContext is ViewRecipeViewModel viewModel)
            {
                viewModel.RecipeID = recipeID;
                try
                {
                    StepsViewer.Document = SQLiteModel.Data.Recipe.ParseSteps(viewModel.MyRecipe.R_Steps);
                }
                catch (Exception e)
                {
                    App.LogException(e);
                    StepsViewer.Document = null;
                }
            }
        }

        private void App_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UnitSystem")
            {
                foreach (object item in IngredientsGrid.Items)
                {
                    if (UnitStringColumn.GetCellContent(item) is TextBlock content)
                    {
                        BindingOperations.GetMultiBindingExpression(content, TextBlock.TextProperty).UpdateTarget();
                    }
                }
            }
        }
    }
}
