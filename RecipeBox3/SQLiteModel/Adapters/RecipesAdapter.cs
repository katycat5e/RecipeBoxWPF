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
    public class RecipesAdapter : SQLiteAdapter<Recipe>
    {
        protected SQLiteParameter nameParameter       = new SQLiteParameter("@name", DbType.String, 50);
        protected SQLiteParameter descParameter       = new SQLiteParameter("@desc", DbType.String, 254);
        protected SQLiteParameter modParameter        = new SQLiteParameter("@modified", DbType.Int64);
        protected SQLiteParameter prepParameter       = new SQLiteParameter("@prep", DbType.Int32);
        protected SQLiteParameter cookParameter       = new SQLiteParameter("@cook", DbType.Int32);
        protected SQLiteParameter stepsParameter      = new SQLiteParameter("@steps", DbType.String);
        protected SQLiteParameter categoryParameter   = new SQLiteParameter("@category", DbType.Int32);

        public RecipesAdapter() : base() { }

        public RecipesAdapter(string connectionString) : base(connectionString) { }

        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);
            
            var dataParams = new SQLiteParameter[]
            {
                nameParameter, descParameter, modParameter,
                prepParameter, cookParameter, stepsParameter, categoryParameter
            };

            SelectCommand.CommandText = "SELECT R_ID, R_Name, R_Description, R_Modified, " +
                "R_PrepTime, R_CookTime, R_Steps, R_Category FROM Recipes WHERE (@id IS NULL) OR (R_ID=@id)";
            SelectCommand.Parameters.Add(idParameter);

            UpdateCommand.CommandText = "UPDATE Recipes SET R_Name=@name, R_Description=@desc, R_Modified=@modified " +
                "R_PrepTime=@prep, R_CookTime=@cook, R_Steps=@steps, R_Category=@category WHERE R_ID=@id";
            UpdateCommand.Parameters.AddRange(dataParams);
            UpdateCommand.Parameters.Add(idParameter);

            InsertCommand.CommandText = "INSERT INTO Recipes (R_Name, R_Description, R_Modified, " +
                "R_PrepTime, R_CookTime, R_Steps, R_Category) " +
                "VALUES (@name, @desc, @modified, @prep, @cook, @steps, @category)";
            InsertCommand.Parameters.AddRange(dataParams);

            DeleteCommand.CommandText = "DELETE FROM Recipes WHERE R_ID=@id";
            DeleteCommand.Parameters.Add(idParameter);
        }

        protected override void SetDataParametersFromRow(Recipe recipe)
        {
            nameParameter.Value = recipe.R_Name;
            descParameter.Value = recipe.R_Description;
            modParameter.Value = recipe.R_Modified;
            prepParameter.Value = recipe.R_PrepTime;
            cookParameter.Value = recipe.R_CookTime;
            stepsParameter.Value = recipe.R_Steps;
            categoryParameter.Value = recipe.R_Category;
        }

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
                Console.WriteLine(e.Message + " at " + e.TargetSite);
                return null;
            }
        }
    }
}
