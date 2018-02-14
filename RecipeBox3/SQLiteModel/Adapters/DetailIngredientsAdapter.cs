using System;
using System.Data.SQLite;
using System.Linq;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Adapter for Ingredients table left joined with Units</summary>
    public class DetailIngredientsAdapter : IngredientsBaseAdapter<DetailIngredient>
    {
        /// <inheritdoc/>
        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);

            SelectCommand.CommandText =
                String.Format(
                    "SELECT {0}, {1}, U_ID, U_Name, U_Plural, U_Abbrev, U_Typecode, U_Ratio, U_System " +
                    "FROM Ingredients LEFT JOIN Units ON IE_Unit=U_ID WHERE (@id IS NULL) OR ({0} = @id)",
                    IDColumn, DataColumns);

            SelectByRecipeCommand.CommandText =
                String.Format(
                    "SELECT {0}, {1}, U_ID, U_Name, U_Plural, U_Abbrev, U_Typecode, U_Ratio, U_System " +
                    "FROM Ingredients LEFT JOIN Units ON IE_Unit=U_ID WHERE IE_RecipeID=@recipe",
                    IDColumn, String.Join(", ", DataColumns));
        }

        /// <inheritdoc/>
        protected override DetailIngredient GetRowFromReader(SQLiteDataReader reader)
        {
            var row = base.GetRowFromReader(reader);
            int columnOffset = base.DataColumns.Count() + 1;

            try
            {
                row.U_ID = reader.GetInt32(columnOffset);
                row.U_Name = reader.GetString(columnOffset + 1);
                row.U_Plural = reader.GetString(columnOffset + 2);
                row.U_Abbreviation = reader.GetString(columnOffset + 3);
                row.U_TypeCode = (Unit.UnitType)reader.GetInt32(columnOffset + 4);
                row.U_Ratio = reader.GetFloat(columnOffset + 5);
                row.U_System = (Unit.System)reader.GetInt32(columnOffset + 6);
            }
            catch (InvalidCastException ex)
            {
                App.LogException(ex);
                return null;
            }

            return row;
        }
    }
}
