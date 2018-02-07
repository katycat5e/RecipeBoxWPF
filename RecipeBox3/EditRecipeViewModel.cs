using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeBox3
{
    public class EditRecipeViewModel : ViewRecipeViewModel
    {
        private CategoriesAdapter categoriesAdapter;
        private UnitsAdapter unitsAdapter;
        
        public Category[] CategoryList
        {
            get { return (Category[])GetValue(CategoryListProperty); }
            set { SetValue(CategoryListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CategoryList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CategoryListProperty =
            DependencyProperty.Register("CategoryList", typeof(Category[]), typeof(EditRecipeViewModel), new PropertyMetadata(null));



        public Unit[] UnitList
        {
            get { return (Unit[])GetValue(UnitListProperty); }
            set
            {
                SetValue(UnitListProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for UnitList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitListProperty =
            DependencyProperty.Register("UnitList", typeof(Unit[]), typeof(EditRecipeViewModel), new PropertyMetadata(null));



        public EditRecipeViewModel() : base()
        {
            categoriesAdapter = new CategoriesAdapter();
            unitsAdapter = new UnitsAdapter();

            CategoryList = categoriesAdapter.SelectAll().ToArray();
            UnitList = unitsAdapter.SelectAll().ToArray();
        }

        /// <summary>
        /// Save the current recipe to the database
        /// </summary>
        /// <returns>true if changes were made to the database</returns>
        public bool SaveRecipe()
        {
            if (MyRecipe.Status == RowStatus.Unchanged)
            {
                MessageBox.Show("No changes detected", "No Change", MessageBoxButton.OK, MessageBoxImage.None);
                return false;
            }

            bool successful = recipesAdapter.Update(MyRecipe);
            if (successful)
            {
                MessageBox.Show("Recipe saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.None);
                return true;
            }
            else
            {
                MessageBox.Show("Recipe could not be saved", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
