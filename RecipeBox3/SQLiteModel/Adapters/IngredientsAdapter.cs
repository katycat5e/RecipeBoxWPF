using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Adapter for the Ingredients table</summary>
    public class IngredientsAdapter : IngredientsBaseAdapter<Ingredient>
    {
        
    }

    /// <summary>Abstract base adapter for the Ingredients table</summary>
    /// <typeparam name="U">Type of row objects returned by this adapter</typeparam>
    public abstract class IngredientsBaseAdapter<U> : SQLiteAdapter<U> where U : Ingredient
    {
        /// <inheritdoc/>
        public override IEnumerable<TableColumn> DataColumns => new TableColumn[]
        {
            new TableColumn("IE_Name", DbType.String, "New Ingredient"),
            new TableColumn("IE_Amount", DbType.String, "0"),
            new TableColumn("IE_Unit", DbType.Int32, 1),
            new TableColumn("IE_RecipeID", DbType.Int32, null)
        };

        /// <inheritdoc/>
        public override string TableName => "Ingredients";
        /// <inheritdoc/>
        public override string IDColumnName => "IE_ID";

        /// <summary>Command to select ingredients based on recipe id</summary>
        protected SQLiteCommand SelectByRecipeCommand;

        /// <summary>Command to select ingredients by matching part of the name</summary>
        protected SQLiteCommand SelectByNameCommand;

        /// <inheritdoc/>
        protected override SQLiteConnection Connection
        {
            get => base.Connection;
            set
            {
                if (value == _connection) return;
                else
                {
                    if (SelectByRecipeCommand != null) SelectByRecipeCommand.Connection = value;
                    if (SelectByNameCommand != null) SelectByNameCommand.Connection = value;
                    base.Connection = value;
                }
            }
        }

        /// <summary>Create a new instance of the class</summary>
        public IngredientsBaseAdapter() : base() { }

        /// <summary>Create a new instance of the class with the specified connection</summary>
        public IngredientsBaseAdapter(string connectionString) : base(connectionString) { }

        /// <inheritdoc/>
        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);

            string recipeParam = TableColumnExtensions.GetParameterName("IE_RecipeID");

            SelectByRecipeCommand = new SQLiteCommand(
                String.Format(
                    "SELECT {0}, {1} FROM {2} WHERE IE_RecipeID={3}",
                    IDColumnName, String.Join(", ", DataColumnNames), TableName, recipeParam),
                Connection);

            if (DataParameters.TryGetValue("IE_RecipeID", out SQLiteParameter recipeParameter))
            {
                SelectByRecipeCommand.Parameters.Add(recipeParameter);
            }

            string nameParam = TableColumnExtensions.GetParameterName("IE_Name");

            SelectByNameCommand = new SQLiteCommand(
                String.Format(
                    "SELECT {0}, {1} FROM {2} WHERE IE_Name LIKE '%' || {3} || '%'",
                    IDColumnName, String.Join(", ", DataColumnNames), TableName, nameParam),
                Connection);

            if (DataParameters.TryGetValue("IE_Name", out SQLiteParameter nameParameter))
                SelectByNameCommand.Parameters.Add(nameParameter);
        }

        /// <summary>Retrieve all ingredients for a recipe</summary>
        /// <param name="recipeID">ID of the recipe to search for</param>
        /// <returns>The set of ingredient objects matching the specified recipe</returns>
        public virtual IEnumerable<U> SelectAllByRecipe(int recipeID)
        {
            if (SelectByRecipeCommand?.Connection == null) return null;
            else
            {
                TrySetParameterValue("IE_RecipeID", recipeID);

                List<U> results = new List<U>();

                using (var reader = ExecuteCommandReader(SelectByRecipeCommand))
                {
                    if (reader.HasRows)
                    {
                        U nextRow;

                        while (reader.Read())
                        {
                            nextRow = GetRowFromReader(reader);
                            if (nextRow != null) results.Add(nextRow);
                        }
                    }
                }

                return results;
            }
        }

        /// <summary>Retrieve all ingredients with a name containing the input</summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IEnumerable<U> SelectAllByName(string name)
        {
            if (SelectByNameCommand?.Connection == null) return null;
            else
            {
                TrySetParameterValue("IE_Name", name);

                List<U> results = new List<U>();

                using (var reader = ExecuteCommandReader(SelectByNameCommand))
                {
                    if (reader.HasRows)
                    {
                        U nextRow;

                        while (reader.Read())
                        {
                            nextRow = GetRowFromReader(reader);
                            if (nextRow != null) results.Add(nextRow);
                        }
                    }
                }

                return results;
            }
        }

        /// <inheritdoc/>
        protected override void SetDataParametersFromRow(U row)
        {
            TrySetParameterValue("IE_Name", row.IE_Name);
            TrySetParameterValue("IE_Amount", row.IE_Amount.ToString());
            TrySetParameterValue("IE_Unit", row.IE_Unit);
        }

        /// <inheritdoc/>
        protected override U GetRowFromReader(SQLiteDataReader reader)
        {
            try
            {
                if (Activator.CreateInstance(typeof(U)) is U ingredient)
                {
                    ingredient.IE_ID = reader.GetInt32(0);
                    ingredient.IE_Name = reader.GetString(1);
                    ingredient.IE_Amount = Fraction.Parse(reader.GetString(2));
                    ingredient.IE_Unit = reader.GetInt32(3);
                    ingredient.IE_RecipeID = reader.GetNullableInt(4);
                    ingredient.Status = RowStatus.Unchanged;

                    return ingredient;
                }
                else return null;
            }
            catch (InvalidCastException ex)
            {
                App.LogException(ex);
                return null;
            }
        }
    }
}
