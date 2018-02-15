using System;
using System.Data;
using System.Data.SQLite;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Adapter for the Units table</summary>
    public class UnitsAdapter : SQLiteAdapter<Unit>
    {
        /// <summary></summary>
        protected SQLiteParameter nameParameter = new SQLiteParameter("@name", DbType.String, "U_Name");
        /// <summary></summary>
        protected SQLiteParameter pluralParameter = new SQLiteParameter("@plural", DbType.String, "U_Plural");
        /// <summary></summary>
        protected SQLiteParameter abbrevParameter = new SQLiteParameter("@abbrev", DbType.String, "U_Abbrev");
        /// <summary></summary>
        protected SQLiteParameter typeParameter = new SQLiteParameter("@typecode", DbType.Int32, "U_Typecode");
        /// <summary></summary>
        protected SQLiteParameter ratioParamter = new SQLiteParameter("@ratio", DbType.Single, "U_Ratio");
        /// <summary></summary>
        protected SQLiteParameter systemParameter = new SQLiteParameter("@system", DbType.String, "U_System");
        /// <summary></summary>
        protected SQLiteParameter editableParameter = new SQLiteParameter("@editable", DbType.Boolean, "U_Editable");

        /// <inheritdoc/>
        protected override SQLiteParameter[] DataParameters =>
            new SQLiteParameter[]
            {
                nameParameter, pluralParameter, abbrevParameter,
                typeParameter, ratioParamter, systemParameter, editableParameter
            };

        /// <inheritdoc/>
        protected override string TableName => "Units";
        /// <inheritdoc/>
        protected override string IDColumn => "U_ID";

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
                    IsUserEditable = reader.GetBoolean(7)
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
            IDParameter.Value = row.U_ID;
            nameParameter.Value = row.U_Name;
            pluralParameter.Value = row.U_Plural;
            abbrevParameter.Value = row.U_Abbreviation;
            typeParameter.Value = (int)row.U_TypeCode;
            ratioParamter.Value = row.U_Ratio;
            systemParameter.Value = (int)row.U_System;
            editableParameter.Value = row.IsUserEditable;
        }
    }
}
