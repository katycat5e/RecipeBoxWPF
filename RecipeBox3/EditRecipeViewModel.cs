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

namespace RecipeBox3
{
    public class EditRecipeViewModel : ViewRecipeViewModel
    {
        private CategoriesAdapter categoriesAdapter;
        
        public Category[] CategoryList
        {
            get { return (Category[])GetValue(CategoryListProperty); }
            set { SetValue(CategoryListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CategoryList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CategoryListProperty =
            DependencyProperty.Register("CategoryList", typeof(Category[]), typeof(EditRecipeViewModel), new PropertyMetadata(null));



        public UnitManager UnitManager
        {
            get { return (UnitManager)GetValue(UnitManagerProperty); }
            set { SetValue(UnitManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitManagerProperty =
            DependencyProperty.Register("UnitManager", typeof(UnitManager), typeof(EditRecipeViewModel), new PropertyMetadata(null));



        public override ObservableCollection<DetailIngredient> Ingredients
        {
            get => base.Ingredients;
            set
            {
                base.Ingredients = value;
                value.CollectionChanged += Ingredients_CollectionChanged;
            }
        }

        private List<DetailIngredient> deletedIngredients = new List<DetailIngredient>();

        public EditRecipeViewModel() : base()
        {
            categoriesAdapter = new CategoriesAdapter();

            CategoryList = categoriesAdapter.SelectAll().ToArray();
        }

        /// <summary>
        /// Save the current recipe to the database
        /// </summary>
        /// <returns>true if changes were made to the database</returns>
        public bool SaveRecipe()
        {
            bool ingredientSuccess = true;
            bool recipeSuccess = true;

            int numIngredientsToUpdate = Ingredients.Where(
                p => (
                    p.Status == RowStatus.New ||
                    p.Status == RowStatus.Modified ||
                    p.Status == RowStatus.Deleted)
                ).Count();

            numIngredientsToUpdate += deletedIngredients.Count;

            int rowsAffected = ingredientsAdapter.Update(Ingredients);
            rowsAffected += ingredientsAdapter.Update(deletedIngredients);

            ingredientSuccess = (rowsAffected == numIngredientsToUpdate);

            if (MyRecipe.Status != RowStatus.Unchanged)
                recipeSuccess = recipesAdapter.Update(MyRecipe);
            else
                recipeSuccess = true;

            if (recipeSuccess)
            {
                string message = "Recipe was saved successfully";
                if (!ingredientSuccess)
                    message += ", but not all ingredient changes could be saved";

                MessageBox.Show(message, "Recipe Saved", MessageBoxButton.OK, MessageBoxImage.None);
                return true;
            }
            else
            {
                MessageBox.Show("Recipe could not be saved", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void Ingredients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DetailIngredient ingred in e.OldItems)
                {
                    if (!deletedIngredients.Contains(ingred))
                    {
                        ingred.Status = RowStatus.Deleted;
                        deletedIngredients.Add(ingred);
                    }
                }
            }
        }
    }
}
