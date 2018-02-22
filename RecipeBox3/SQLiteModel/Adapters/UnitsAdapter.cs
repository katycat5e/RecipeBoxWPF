using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Adapter for the Units table</summary>
    public class UnitsAdapter : SQLiteAdapter<Unit>
    {
        /// <inheritdoc/>
        public override IEnumerable<TableColumn> DataColumns => new TableColumn[]
        {
            new TableColumn("U_Name", DbType.String, "New Unit", ColumnOptions.NotNull | ColumnOptions.Unique),
            new TableColumn("U_Plural", DbType.String, null),
            new TableColumn("U_Abbrev", DbType.String, "nu", ColumnOptions.NotNull | ColumnOptions.Unique),
            new TableColumn("U_Typecode", DbType.Int32, Unit.UnitType.Mass),
            new TableColumn("U_Ratio", DbType.Single, 1.0F),
            new TableColumn("U_System", DbType.Int32, Unit.System.Any),
            new TableColumn("U_Editable", DbType.Boolean, true)
        };

        /// <inheritdoc/>
        public override string TableName => "Units";
        /// <inheritdoc/>
        public override string IDColumnName => "U_ID";

        /// <inheritdoc/>
        protected override Unit GetRowFromReader(SQLiteDataReader reader)
        {
            try
            {
                Unit unit = new Unit()
                {
                    U_ID = reader.GetInt32(0),
                    U_Name = reader.GetString(1),
                    U_Plural = reader.GetString(2),
                    U_Abbreviation = reader.GetString(3),
                    U_TypeCode = (Unit.UnitType)reader.GetInt32(4),
                    U_Ratio = reader.GetFloat(5),
                    U_System = (Unit.System)reader.GetInt32(6),
                    IsUserEditable = reader.GetBoolean(7),
                    Status = RowStatus.Unchanged,
                };

                return unit;
            }
            catch (InvalidCastException ex)
            {
                App.LogException(ex);
                return null;
            }
        }

        /// <inheritdoc/>
        protected override void SetDataParametersFromRow(Unit row)
        {
            TrySetParameterValue("U_Name", row.U_Name);
            TrySetParameterValue("U_Plural", row.U_Plural);
            TrySetParameterValue("U_Abbrev", row.U_Abbreviation);
            TrySetParameterValue("U_Typecode", (int)row.U_TypeCode);
            TrySetParameterValue("U_Ratio", row.U_Ratio);
            TrySetParameterValue("U_System", (int)row.U_System);
            TrySetParameterValue("U_Editable", row.IsUserEditable);
        }
    }
}
