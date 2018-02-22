using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Adapter for the Recipes table</summary>
    public class RecipesAdapter : RecipesBaseAdapter<Recipe>
    {
        
    }

    /// <summary>Abstract base adapter for the Recipes table</summary>
    /// <typeparam name="U">Type of row objects returned by this adapter</typeparam>
    public abstract class RecipesBaseAdapter<U> : SQLiteAdapter<U> where U : Recipe
    {
        /// <inheritdoc/>
        public override string TableName => "Recipes";
        /// <inheritdoc/>
        public override string IDColumnName => "R_ID";

        /// <inheritdoc/>
        public override IEnumerable<TableColumn> DataColumns => new TableColumn[]
        {
            new TableColumn("R_Name", DbType.String, "New Recipe", ColumnOptions.NotNull | ColumnOptions.Unique),
            new TableColumn("R_Description", DbType.String, ""),
            new TableColumn("R_Modified", DbType.Int64, null),
            new TableColumn("R_PrepTime", DbType.Int32, 0),
            new TableColumn("R_CookTime", DbType.Int32, 0),
            new TableColumn("R_Steps", DbType.String, ""),
            new TableColumn("R_Category", DbType.Int32, 1)
        };

        /// <summary>Create a new instance of the class</summary>
        public RecipesBaseAdapter() : base() { }

        /// <summary>Create a new instance of the class with the specified connection</summary>
        public RecipesBaseAdapter(string connectionString) : base(connectionString) { }

        /// <inheritdoc/>
        protected override void SetDataParametersFromRow(U recipe)
        {
            TrySetParameterValue("R_Name", recipe.R_Name);
            TrySetParameterValue("R_Description", recipe.R_Description);
            TrySetParameterValue("R_Modified", recipe.R_Modified);
            TrySetParameterValue("R_PrepTime", recipe.R_PrepTime);
            TrySetParameterValue("R_CookTime", recipe.R_CookTime);
            TrySetParameterValue("R_Steps", recipe.R_Steps);
            TrySetParameterValue("R_Category", recipe.R_Category);
        }

        /// <inheritdoc/>
        protected override U GetRowFromReader(SQLiteDataReader reader)
        {
            try
            {
                if (Activator.CreateInstance(typeof(U)) is U recipe)
                {
                    recipe.R_ID = reader.GetInt32(0);
                    recipe.R_Name = reader.GetString(1);
                    recipe.R_Description = reader.GetString(2);
                    recipe.R_Modified = reader.GetValue(3) as long?;
                    recipe.R_PrepTime = reader.GetInt32(4);
                    recipe.R_CookTime = reader.GetInt32(5);
                    recipe.R_Steps = reader.GetString(6);
                    recipe.R_Category = reader.GetInt32(7);
                    recipe.Status = RowStatus.Unchanged;

                    return recipe;
                }
                else return null;
            }
            catch (InvalidCastException e)
            {
                App.LogException(e);
                return null;
            }
        }
    }
}
