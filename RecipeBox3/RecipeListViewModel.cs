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

        public ObservableCollection<DetailRecipe> Recipes
        {
            get { return (ObservableCollection<DetailRecipe>)GetValue(RecipesProperty); }
            set { SetValue(RecipesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Recipes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecipesProperty =
            DependencyProperty.Register("Recipes", typeof(ObservableCollection<DetailRecipe>), typeof(RecipeListViewModel), new PropertyMetadata(null));


        public object SelectedGridItem
        {
            get { return (object)GetValue(SelectedGridItemProperty); }
            set { SetValue(SelectedGridItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedGridItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedGridItemProperty =
            DependencyProperty.Register("SelectedGridItem", typeof(object), typeof(RecipeListViewModel), new PropertyMetadata(null));

        
        public bool ShowImages
        {
            get { return (bool)GetValue(ShowImagesProperty); }
            set
            {
                SetValue(ShowImagesProperty, value);
                recipesAdapter.RetrieveImages = value;
            }
        }

        // Using a DependencyProperty as the backing store for ShowImages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowImagesProperty =
            DependencyProperty.Register("ShowImages", typeof(bool), typeof(RecipeListViewModel), new PropertyMetadata(true));



        public RecipeListViewModel()
        {
            recipesAdapter = new DetailRecipesAdapter();
            GetAllRecipes();
        }

        public void GetAllRecipes()
        {
            Recipes = new ObservableCollection<DetailRecipe>(recipesAdapter.SelectAll());
        }

        public void DeleteRecipe(int id)
        {
            recipesAdapter.Delete(id);
        }
    }
}
