using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace RecipeBox3
{
    /// <summary>View model for recipe editor</summary>
    public class EditRecipeViewModel : ViewRecipeViewModel
    {
        private CategoriesAdapter categoriesAdapter;
        
        /// <summary>List of categories for the category drop-down</summary>
        public Category[] CategoryList
        {
            get { return (Category[])GetValue(CategoryListProperty); }
            set { SetValue(CategoryListProperty, value); }
        }

        /// <summary>List of categories for the category drop-down</summary>
        public static readonly DependencyProperty CategoryListProperty =
            DependencyProperty.Register("CategoryList", typeof(Category[]), typeof(EditRecipeViewModel), new PropertyMetadata(null));


        /// <summary>Unit manager for ingredient editing</summary>
        public UnitManager UnitManager
        {
            get { return (UnitManager)GetValue(UnitManagerProperty); }
            set { SetValue(UnitManagerProperty, value); }
        }

        /// <summary>Unit manager for ingredient editing</summary>
        public static readonly DependencyProperty UnitManagerProperty =
            DependencyProperty.Register("UnitManager", typeof(UnitManager), typeof(EditRecipeViewModel), new PropertyMetadata(null));


        /// <inheritdoc/>
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

        /// <summary>Create a new instance of the class</summary>
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

            string newSteps = Recipe.SerializeSteps(StepsDocument);
            if (newSteps != MyRecipe.R_Steps)
                MyRecipe.R_Steps = newSteps;

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
