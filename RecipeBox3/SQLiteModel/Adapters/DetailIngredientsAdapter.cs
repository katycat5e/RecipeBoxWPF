using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    public class DetailIngredientsAdapter : IngredientsBaseAdapter<DetailIngredient>
    {
        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);

            SelectCommand.CommandText =
                String.Format(
                    "SELECT {0}, {1}, U_Name, U_Plural, U_Abbrev, U_Typecode, U_Ratio, U_System " +
                    "FROM Ingredients LEFT JOIN Units ON IE_Unit=U_ID WHERE (@id IS NULL) OR ({0} = @id)",
                    IDColumn, DataColumns);
        }

        protected override DetailIngredient GetRowFromReader(SQLiteDataReader reader)
        {
            var row = base.GetRowFromReader(reader);
            int columnOffset = base.DataColumns.Count() + 1;

            try
            {
                row.U_Name = reader.GetString(columnOffset);
                row.U_Plural = reader.GetString(columnOffset + 1);
                row.U_Abbreviation = reader.GetString(columnOffset + 2);
                row.U_TypeCode = (Unit.UnitType)reader.GetInt32(columnOffset + 3);
                row.U_Ratio = reader.GetFloat(columnOffset + 4);
                row.U_System = (Unit.System)reader.GetInt32(columnOffset + 5);
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
