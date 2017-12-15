using System.Collections.Generic;
using System.Windows;

namespace RecipeBox3
{
    public class SearchItems : DependencyObject
    {
        public string SearchName
        {
            get { return (string)GetValue(SearchNameProperty); }
            set { SetValue(SearchNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchNameProperty =
            DependencyProperty.Register("SearchName", typeof(string), typeof(SearchItems), new PropertyMetadata(string.Empty));



        public string SearchIngredients
        {
            get { return (string)GetValue(SearchIngredientsProperty); }
            set { SetValue(SearchIngredientsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchIngredients.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchIngredientsProperty =
            DependencyProperty.Register("SearchIngredients", typeof(string), typeof(SearchItems), new PropertyMetadata(string.Empty));



        public int SearchMaxTime
        {
            get { return (int)GetValue(SearchMaxTimeProperty); }
            set { SetValue(SearchMaxTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchMaxTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchMaxTimeProperty =
            DependencyProperty.Register("SearchMaxTime", typeof(int), typeof(SearchItems), new PropertyMetadata(0));



        public int SearchCategory
        {
            get { return (int)GetValue(SearchCategoryProperty); }
            set { SetValue(SearchCategoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchCategory.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchCategoryProperty =
            DependencyProperty.Register("SearchCategory", typeof(int), typeof(SearchItems), new PropertyMetadata(0));



        public Dictionary<string, int> CategoryOptions
        {
            get { return (Dictionary<string, int>)GetValue(CategoryOptionsProperty); }
            set { SetValue(CategoryOptionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CategoryOptions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CategoryOptionsProperty =
            DependencyProperty.Register("CategoryOptions", typeof(Dictionary<string, int>), typeof(SearchItems), new PropertyMetadata(new Dictionary<string, int>()));
        

    }
}
