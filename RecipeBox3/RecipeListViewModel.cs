using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeBox3
{
    public class RecipeListViewModel : DependencyObject
    {
        private DetailRecipesAdapter recipesAdapter;
        private CategoriesAdapter categoriesAdapter;

        public ObservableCollection<DetailRecipe> Recipes
        {
            get { return (ObservableCollection<DetailRecipe>)GetValue(RecipesProperty); }
            set { SetValue(RecipesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Recipes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecipesProperty =
            DependencyProperty.Register("Recipes", typeof(ObservableCollection<DetailRecipe>), typeof(RecipeListViewModel), new PropertyMetadata(null));


        public RecipeListViewModel()
        {
            recipesAdapter = new DetailRecipesAdapter();
            categoriesAdapter = new CategoriesAdapter();
            GetAllRecipes();
        }

        public void GetAllRecipes()
        {
            Recipes = new ObservableCollection<DetailRecipe>(recipesAdapter.SelectAll());
        }
    }
}
