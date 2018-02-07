﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    public class IngredientsAdapter : IngredientsBaseAdapter<Ingredient>
    {
        
    }

    public abstract class IngredientsBaseAdapter<U> : SQLiteAdapter<U> where U : IngredientBase<U>
    {
        protected SQLiteParameter nameParameter = new SQLiteParameter("@name", DbType.String, "IE_Name");
        protected SQLiteParameter amountParameter = new SQLiteParameter("@amount", DbType.Decimal, "IE_Amount");
        protected SQLiteParameter unitParameter = new SQLiteParameter("@unit", DbType.Int32, "IE_Unit");
        protected SQLiteParameter recipeParameter = new SQLiteParameter("@recipe", DbType.Int32, "IE_RecipeID");

        protected override SQLiteParameter[] DataParameters
        {
            get
            {
                return new SQLiteParameter[]
                {
                    nameParameter, amountParameter, unitParameter, recipeParameter
                };
            }
        }

        protected override string TableName => "Ingredients";
        protected override string IDColumn => "IE_ID";

        protected SQLiteCommand SelectByRecipeCommand;

        protected override SQLiteConnection Connection
        {
            get => base.Connection;
            set
            {
                if (value == _connection) return;
                else
                {
                    if (SelectByRecipeCommand != null) SelectByRecipeCommand.Connection = value;
                    base.Connection = value;
                }
            }
        }

        public IngredientsBaseAdapter() : base() { }

        public IngredientsBaseAdapter(string connectionString) : base(connectionString) { }

        /// <inheritdoc/>
        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);

            SelectByRecipeCommand = new SQLiteCommand(
                String.Format(
                    "SELECT {0}, {1} FROM {2} WHERE IE_RecipeID=@recipeid",
                    IDColumn, String.Join(", ", DataColumns), TableName),
                Connection);
            SelectByRecipeCommand.Parameters.Add(recipeParameter);
        }

        /// <summary>Retrieve all ingredients for a recipe</summary>
        /// <param name="recipeID">ID of the recipe to search for</param>
        /// <returns>The set of ingredient objects matching the specified recipe</returns>
        public virtual IEnumerable<U> SelectAllByRecipe(int recipeID)
        {
            if (SelectByRecipeCommand?.Connection == null) return null;
            else
            {
                recipeParameter.Value = recipeID;

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

        /// <inheritdoc/>
        protected override void SetDataParametersFromRow(U row)
        {
            nameParameter.Value = row.IE_Name;
            amountParameter.Value = row.IE_Amount;
            unitParameter.Value = row.IE_Unit;
        }

        protected override U GetRowFromReader(SQLiteDataReader reader)
        {
            try
            {
                U ingredient = Activator.CreateInstance(typeof(U)) as U;

                if (ingredient != null)
                {
                    ingredient.IE_ID = reader.GetInt32(0);
                    ingredient.IE_Name = reader.GetString(1);
                    ingredient.IE_Amount = reader.GetDecimal(2);
                    ingredient.IE_Unit = reader.GetInt32(3);
                    ingredient.IE_RecipeID = reader.GetValue(4) as int?;
                }

                return ingredient;
            }
            catch (InvalidCastException ex)
            {
                App.LogException(ex);
                return null;
            }
        }
    }
}
