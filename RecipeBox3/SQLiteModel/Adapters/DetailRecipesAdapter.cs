using RecipeBox3.SQLiteModel.Data;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox3.SQLiteModel.Adapters
{
    class DetailRecipesAdapter : SQLiteAdapter<DetailRecipe>
    {
        private RecipesAdapter recipesAdapter;
        private CategoriesAdapter categoriesAdapter;

        public bool RetrieveImages = false;

        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);

            recipesAdapter = new RecipesAdapter();
            categoriesAdapter = new CategoriesAdapter();
        }

        public override DetailRecipe Select(int id)
        {
            var recipe = recipesAdapter.Select(id);
            if (recipe == null) return null;

            var tmpCategory = categoriesAdapter.Select(recipe.R_Category);
            return new DetailRecipe(recipe)
            {
                C_Name = tmpCategory?.C_Name
            };
        }

        public override IEnumerable<DetailRecipe> SelectAll()
        {
            var recipes = recipesAdapter.SelectAll();

            var results = recipes.Select(p => new DetailRecipe(p)).ToList();

            Category tmpCategory;
            foreach (DetailRecipe recipe in results)
            {
                tmpCategory = categoriesAdapter.Select(recipe.R_Category);
                recipe.C_Name = tmpCategory?.C_Name;
            }

            return results;
        }

        public override bool Modify(DetailRecipe row)
        {
            return recipesAdapter.Modify(new Recipe(row));
        }

        public override bool Insert(DetailRecipe row)
        {
            var newRow = new Recipe(row);

            return recipesAdapter.Insert(newRow);
        }

        public override bool Delete(int id)
        {
            return recipesAdapter.Delete(id);
        }

        public override bool Delete(DetailRecipe row)
        {
            return recipesAdapter.Delete(new Recipe(row));
        }
    }
}
