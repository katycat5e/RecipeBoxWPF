using System;
using System.Data;
using System.Data.SQLite;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Adapter for the Recipes table</summary>
    public class RecipesAdapter : RecipesBaseAdapter<Recipe>
    {
        /// <inheritdoc/>
        protected override Recipe GetRowFromReader(SQLiteDataReader reader)
        {
            try
            {
                Recipe nextRow = new Recipe()
                {
                    R_ID = reader.GetInt32(0),
                    R_Name = reader.GetString(1),
                    R_Description = reader.GetString(2),
                    R_Modified = reader.GetValue(3) as long?,
                    R_PrepTime = reader.GetInt32(4),
                    R_CookTime = reader.GetInt32(5),
                    R_Steps = reader.GetString(6),
                    R_Category = reader.GetInt32(7),
                    Status = RowStatus.Unchanged
                };

                return nextRow;
            }
            catch (InvalidCastException e)
            {
                App.LogException(e);
                return null;
            }
        }
    }

    /// <summary>Abstract base adapter for the Recipes table</summary>
    /// <typeparam name="U">Type of row objects returned by this adapter</typeparam>
    public abstract class RecipesBaseAdapter<U> : SQLiteAdapter<U> where U : Recipe
    {
        /// <summary></summary>
        protected SQLiteParameter nameParameter       = new SQLiteParameter("@name", DbType.String, 50, "R_Name");
        /// <summary></summary>
        protected SQLiteParameter descParameter       = new SQLiteParameter("@desc", DbType.String, 254, "R_Description");
        /// <summary></summary>
        protected SQLiteParameter modParameter        = new SQLiteParameter("@modified", DbType.Int64, "R_Modified");
        /// <summary></summary>
        protected SQLiteParameter prepParameter       = new SQLiteParameter("@prep", DbType.Int32, "R_PrepTime");
        /// <summary></summary>
        protected SQLiteParameter cookParameter       = new SQLiteParameter("@cook", DbType.Int32, "R_CookTime");
        /// <summary></summary>
        protected SQLiteParameter stepsParameter      = new SQLiteParameter("@steps", DbType.String, "R_Steps");
        /// <summary></summary>
        protected SQLiteParameter categoryParameter   = new SQLiteParameter("@category", DbType.Int32, "R_Category");

        /// <inheritdoc/>
        protected override string TableName => "Recipes";
        /// <inheritdoc/>
        protected override string IDColumn => "R_ID";

        /// <inheritdoc/>
        protected override SQLiteParameter[] DataParameters => new SQLiteParameter[]
            {
                nameParameter, descParameter, modParameter,
                prepParameter, cookParameter, stepsParameter,
                categoryParameter
            };

        /// <summary>Create a new instance of the class</summary>
        public RecipesBaseAdapter() : base() { }

        /// <summary>Create a new instance of the class with the specified connection</summary>
        public RecipesBaseAdapter(string connectionString) : base(connectionString) { }

        /// <inheritdoc/>
        protected override void SetDataParametersFromRow(U recipe)
        {
            nameParameter.Value = recipe.R_Name;
            descParameter.Value = recipe.R_Description;
            modParameter.Value = recipe.R_Modified;
            prepParameter.Value = recipe.R_PrepTime;
            cookParameter.Value = recipe.R_CookTime;
            stepsParameter.Value = recipe.R_Steps;
            categoryParameter.Value = recipe.R_Category;
        }
    }
}
