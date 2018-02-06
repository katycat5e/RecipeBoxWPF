using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
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

    public abstract class RecipesBaseAdapter<U> : SQLiteAdapter<U> where U : RecipeBase<U>
    {
        protected SQLiteParameter nameParameter       = new SQLiteParameter("@name", DbType.String, 50, "R_Name");
        protected SQLiteParameter descParameter       = new SQLiteParameter("@desc", DbType.String, 254, "R_Description");
        protected SQLiteParameter modParameter        = new SQLiteParameter("@modified", DbType.Int64, "R_Modified");
        protected SQLiteParameter prepParameter       = new SQLiteParameter("@prep", DbType.Int32, "R_PrepTime");
        protected SQLiteParameter cookParameter       = new SQLiteParameter("@cook", DbType.Int32, "R_CookTime");
        protected SQLiteParameter stepsParameter      = new SQLiteParameter("@steps", DbType.String, "R_Steps");
        protected SQLiteParameter categoryParameter   = new SQLiteParameter("@category", DbType.Int32, "R_Category");
        
        protected override string TableName => "Recipes";
        protected override string IDColumn => "R_ID";

        public RecipesBaseAdapter() : base() { }

        public RecipesBaseAdapter(string connectionString) : base(connectionString) { }

        protected override void InitializeDataParameters()
        {
            DataParameters = new SQLiteParameter[]
            {
                nameParameter, descParameter, modParameter,
                prepParameter, cookParameter, stepsParameter,
                categoryParameter
            };
        }

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
