using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RecipeBox3.SQLiteModel.Data;
using RecipeBox3.SQLiteModel.Adapters;
using System.Collections.ObjectModel;

namespace RecipeBox3
{
    public class ViewRecipeViewModel : DependencyObject
    {
        protected DetailRecipesAdapter recipesAdapter;
        protected DetailIngredientsAdapter ingredientsAdapter;

        public int? RecipeID
        {
            get { return MyRecipe?.ID; }
            set
            {
                if (value != MyRecipe?.ID)
                {
                    if (value.HasValue)
                    {
                        MyRecipe = recipesAdapter.SelectWithImage(value.Value);
                        Ingredients = new ObservableCollection<DetailIngredient>(ingredientsAdapter.SelectAllByRecipe(value.Value));
                    }
                    else
                    {
                        MyRecipe = null;
                        Ingredients = null;
                    }
                }
            }
        }

        public DetailRecipe MyRecipe
        {
            get { return (DetailRecipe)GetValue(MyRecipeProperty); }
            set { SetValue(MyRecipeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyRecipe.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyRecipeProperty =
            DependencyProperty.Register("MyRecipe", typeof(DetailRecipe), typeof(ViewRecipeViewModel), new PropertyMetadata(null));



        public virtual ObservableCollection<DetailIngredient> Ingredients
        {
            get { return (ObservableCollection<DetailIngredient>)GetValue(IngredientsProperty); }
            set { SetValue(IngredientsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ingredients.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IngredientsProperty =
            DependencyProperty.Register("Ingredients", typeof(ObservableCollection<DetailIngredient>), typeof(ViewRecipeViewModel), new PropertyMetadata(new ObservableCollection<DetailIngredient>()));



        public ViewRecipeViewModel()
        {
            recipesAdapter = new DetailRecipesAdapter();
            ingredientsAdapter = new DetailIngredientsAdapter();
        }
    }
}
