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
    public abstract class IngredientsBaseAdapter<U> : SQLiteAdapter<U> where U : IngredientBase<U>
    {
        protected SQLiteParameter nameParameter = new SQLiteParameter("@name", DbType.String, "IE_Name");
        protected SQLiteParameter amountParameter = new SQLiteParameter("@amount", DbType.Decimal, "IE_Amount");
        protected SQLiteParameter unitParameter = new SQLiteParameter("@unit", DbType.Int32, "IE_Unit");
        
        

        protected override string TableName => throw new NotImplementedException();

        public IngredientsBaseAdapter()
        {
            DataParameters = new SQLiteParameter[]
            {
                nameParameter, amountParameter, unitParameter
            };
        }

        protected override Ingredient GetRowFromReader(SQLiteDataReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void SetDataParametersFromRow(Ingredient row)
        {
            throw new NotImplementedException();
        }
    }
}
