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
        
        public Category[] CategoryList
        {
            get { return (Category[])GetValue(CategoryListProperty); }
            set { SetValue(CategoryListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CategoryList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CategoryListProperty =
            DependencyProperty.Register("CategoryList", typeof(Category[]), typeof(EditRecipeViewModel), new PropertyMetadata(null));



        public EditRecipeViewModel() : base()
        {
            categoriesAdapter = new CategoriesAdapter();
            CategoryList = categoriesAdapter.SelectAll().ToArray();
        }
    }
}
