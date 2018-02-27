using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3
{
    /// <summary>Category caching class</summary>
    public class CategoryManager : StaticTableManager<CategoriesAdapter, Category>
    {
        /// <summary>Create a new instance of the class</summary>
        public CategoryManager() : base()
        {
            
        }
    }
}
