using System.Collections.Generic;
using System.Windows;

namespace RecipeBox3
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


        /// <summary>Set of categories for the category selector</summary>
        public Dictionary<string, int> CategoryOptions
        {
            get { return (Dictionary<string, int>)GetValue(CategoryOptionsProperty); }
            set { SetValue(CategoryOptionsProperty, value); }
        }

        /// <summary>Set of categories for the category selector</summary>
        public static readonly DependencyProperty CategoryOptionsProperty =
            DependencyProperty.Register("CategoryOptions", typeof(Dictionary<string, int>), typeof(SearchItems), new PropertyMetadata(new Dictionary<string, int>()));
        

    }
}
