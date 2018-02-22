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
using System.Windows.Documents;

namespace RecipeBox3
{
    /// <summary>View model for the recipe viewer</summary>
    public class ViewRecipeViewModel : DependencyObject
    {
        /// <summary>Recipes database adapter</summary>
        protected DetailRecipesAdapter recipesAdapter;
        /// <summary>Ingredients database adapter</summary>
        protected DetailIngredientsAdapter ingredientsAdapter;

        /// <summary>Get the ID of the current recipe, or load a different recipe by ID</summary>
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
                        if (MyRecipe?.R_Steps != null)
                        {
                            StepsDocument = Recipe.ParseSteps(MyRecipe.R_Steps);
                        }
                    }
                    else
                    {
                        MyRecipe = null;
                        Ingredients = null;
                    }
                }
            }
        }

        /// <summary>Currently loaded recipe</summary>
        public DetailRecipe MyRecipe
        {
            get { return (DetailRecipe)GetValue(MyRecipeProperty); }
            set { SetValue(MyRecipeProperty, value); }
        }

        /// <summary>Currently loaded recipe</summary>
        public static readonly DependencyProperty MyRecipeProperty =
            DependencyProperty.Register("MyRecipe", typeof(DetailRecipe), typeof(ViewRecipeViewModel), new PropertyMetadata(null));


        /// <summary>Collection of ingredients associated with the current recipe</summary>
        public virtual ObservableCollection<DetailIngredient> Ingredients
        {
            get { return (ObservableCollection<DetailIngredient>)GetValue(IngredientsProperty); }
            set { SetValue(IngredientsProperty, value); }
        }

        /// <summary>Collection of ingredients associated with the current recipe</summary>
        public static readonly DependencyProperty IngredientsProperty =
            DependencyProperty.Register("Ingredients", typeof(ObservableCollection<DetailIngredient>), typeof(ViewRecipeViewModel), new PropertyMetadata(new ObservableCollection<DetailIngredient>()));
        
        
        /// <summary>System of units to use for ingredient display</summary>
        public Unit.System UnitSystem
        {
            get { return (Unit.System)GetValue(UnitSystemProperty); }
            set { SetValue(UnitSystemProperty, value); }
        }

        /// <summary>Property store for <see cref='UnitSystem'/></summary>
        public static readonly DependencyProperty UnitSystemProperty =
            DependencyProperty.Register("UnitSystem", typeof(Unit.System), typeof(ViewRecipeViewModel), new PropertyMetadata(Unit.System.Any));
        

        /// <summary>Steps for the current recipe in viewable/editable form</summary>
        public FlowDocument StepsDocument
        {
            get { return (FlowDocument)GetValue(StepsDocumentProperty); }
            set { SetValue(StepsDocumentProperty, value); }
        }

        /// <summary>Steps for the current recipe in viewable/editable form</summary>
        public static readonly DependencyProperty StepsDocumentProperty =
            DependencyProperty.Register("StepsDocument", typeof(FlowDocument), typeof(ViewRecipeViewModel), new PropertyMetadata(null));


        /// <summary>Create a new instance of the class</summary>
        public ViewRecipeViewModel()
        {
            recipesAdapter = new DetailRecipesAdapter();
            ingredientsAdapter = new DetailIngredientsAdapter();
        }
    }
}
