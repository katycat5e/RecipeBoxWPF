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
    public class UnitsAdapter : SQLiteAdapter<Unit>
    {
        protected SQLiteParameter nameParameter = new SQLiteParameter("@name", DbType.String, "U_Name");
        protected SQLiteParameter pluralParameter = new SQLiteParameter("@plural", DbType.String, "U_Plural");
        protected SQLiteParameter abbrevParameter = new SQLiteParameter("@abbrev", DbType.String, "U_Abbrev");
        protected SQLiteParameter typeParameter = new SQLiteParameter("@typecode", DbType.Int32, "U_Typecode");
        protected SQLiteParameter ratioParamter = new SQLiteParameter("@ratio", DbType.Single, "U_Ratio");
        protected SQLiteParameter systemParameter = new SQLiteParameter("@system", DbType.String, "U_System");
        protected SQLiteParameter editableParameter = new SQLiteParameter("@editable", DbType.Boolean, "U_Editable");

        protected override SQLiteParameter[] DataParameters =>
            new SQLiteParameter[]
            {
                nameParameter, pluralParameter, abbrevParameter,
                typeParameter, ratioParamter, systemParameter, editableParameter
            };

        protected override string TableName => "Units";
        protected override string IDColumn => "U_ID";

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
