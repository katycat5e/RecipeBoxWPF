using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.Windows
{
    /// <summary>Search properties for the <see cref="RecipeListWindow"/></summary>
    public class SearchItems : DependencyObject
    {
        /// <summary>Name to search for</summary>
        public string SearchName
        {
            get { return (string)GetValue(SearchNameProperty); }
            set { SetValue(SearchNameProperty, value); }
        }

        /// <summary>Name to search for</summary>
        public static readonly DependencyProperty SearchNameProperty =
            DependencyProperty.Register("SearchName", typeof(string), typeof(SearchItems), new PropertyMetadata(string.Empty));


        /// <summary>Ingredients to search for</summary>
        public string SearchIngredients
        {
            get { return (string)GetValue(SearchIngredientsProperty); }
            set { SetValue(SearchIngredientsProperty, value); }
        }

        /// <summary>Ingredients to search for</summary>
        public static readonly DependencyProperty SearchIngredientsProperty =
            DependencyProperty.Register("SearchIngredients", typeof(string), typeof(SearchItems), new PropertyMetadata(string.Empty));


        /// <summary>Maximum time to filter by</summary>
        public int SearchMaxTime
        {
            get { return (int)GetValue(SearchMaxTimeProperty); }
            set { SetValue(SearchMaxTimeProperty, value); }
        }

        /// <summary>Maximum time to filter by</summary>
        public static readonly DependencyProperty SearchMaxTimeProperty =
            DependencyProperty.Register("SearchMaxTime", typeof(int), typeof(SearchItems), new PropertyMetadata(0));


        /// <summary>Category to filter by</summary>
        public int SearchCategory
        {
            get { return (int)GetValue(SearchCategoryProperty); }
            set { SetValue(SearchCategoryProperty, value); }
        }

        /// <summary>Category to filter by</summary>
        public static readonly DependencyProperty SearchCategoryProperty =
            DependencyProperty.Register("SearchCategory", typeof(int), typeof(SearchItems), new PropertyMetadata(0));


        /// <summary></summary>
        public CategoryManager CategoryManager
        {
            get { return (CategoryManager)GetValue(CategoryManagerProperty); }
            set { SetValue(CategoryManagerProperty, value); }
        }

        /// <summary>Property store for <see cref='CategoryManager'/></summary>
        public static readonly DependencyProperty CategoryManagerProperty =
            DependencyProperty.Register("CategoryManager", typeof(CategoryManager), typeof(SearchItems), new PropertyMetadata(null));


        /// <summary>Apply search parameters to a set of recipes</summary>
        /// <param name="recipes">Set of recipes to filter</param>
        /// <returns></returns>
        public List<DetailRecipe> FilterRecipes(IEnumerable<DetailRecipe> recipes)
        {
            IEnumerable<DetailRecipe> outputEnumerable = recipes;

            if (!string.IsNullOrWhiteSpace(SearchName))
            {
                outputEnumerable = outputEnumerable.Where(
                    recipe => recipe.R_Name.Contains(SearchName));
            }

            if (!string.IsNullOrWhiteSpace(SearchIngredients))
            {
                var ingredAdapter = new IngredientsAdapter();
                string[] names = SearchIngredients.Split(',');
                IEnumerable<int> matchingRecipes = null;

                for (int i = 0; i < names.Length; i++)
                {
                    List<int> tempMatch =
                        ingredAdapter.SelectAllByName(names[i])
                            .Select(ingred => ingred.IE_RecipeID)
                            .Where(id => id.HasValue)
                            .Select(id => id.Value)
                            .Distinct().ToList();

                    if (matchingRecipes == null) matchingRecipes = tempMatch;
                    else matchingRecipes = matchingRecipes.Intersect(tempMatch);
                }

                outputEnumerable = outputEnumerable.Where(recipe => matchingRecipes.Contains(recipe.ID));
            }

            if (SearchMaxTime > 0)
            {
                outputEnumerable = outputEnumerable.Where(recipe => recipe.R_PrepTime + recipe.R_CookTime < SearchMaxTime);
            }

            if (SearchCategory > 0)
            {
                outputEnumerable = outputEnumerable.Where(recipe => recipe.R_Category == SearchCategory);
            }
            
            return outputEnumerable.ToList();
        }

        /// <summary>Clear all search properties</summary>
        public void ClearParameters()
        {
            SearchName = "";
            SearchIngredients = "";
            SearchMaxTime = 0;
            SearchCategory = 0;
        }
    }
}
