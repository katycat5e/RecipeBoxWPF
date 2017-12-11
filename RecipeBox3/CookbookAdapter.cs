using RecipeBox3.CookbookDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox3
{
    public class CookbookAdapter
    {
        public RecipesTableAdapter RecipesTableAdapter { get; }
        public ImagesTableAdapter ImagesTableAdapter { get; }
        public IngredientsTableAdapter IngredientsTableAdapter { get; }
        public UnitsTableAdapter UnitsTableAdapter { get; }
        public CategoriesTableAdapter CategoriesTableAdapter { get; }
        public SimpleRecipeViewTableAdapter SimpleRecipeViewTableAdapter { get; }
        public SimpleIngredientsViewTableAdapter SimpleIngredientsViewTableAdapter { get; }
        public QueriesTableAdapter QueriesTableAdapter { get; }

        public CookbookAdapter()
        {
            RecipesTableAdapter = new RecipesTableAdapter();
            ImagesTableAdapter = new ImagesTableAdapter();
            IngredientsTableAdapter = new IngredientsTableAdapter();
            UnitsTableAdapter = new UnitsTableAdapter();
            CategoriesTableAdapter = new CategoriesTableAdapter();
            SimpleRecipeViewTableAdapter = new SimpleRecipeViewTableAdapter();
            SimpleIngredientsViewTableAdapter = new SimpleIngredientsViewTableAdapter();
            QueriesTableAdapter = new QueriesTableAdapter();
        }

        public void SetConnectionString(string connString)
        {
            RecipesTableAdapter.Connection.ConnectionString = connString;
            ImagesTableAdapter.Connection.ConnectionString = connString;
            IngredientsTableAdapter.Connection.ConnectionString = connString;
            UnitsTableAdapter.Connection.ConnectionString = connString;
            CategoriesTableAdapter.Connection.ConnectionString = connString;
            SimpleRecipeViewTableAdapter.Connection.ConnectionString = connString;
            SimpleIngredientsViewTableAdapter.Connection.ConnectionString = connString;
            QueriesTableAdapter.SetConnectionString(connString);
        }
    }
}
