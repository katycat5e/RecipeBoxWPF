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
    public sealed class CategoriesAdapter : SQLiteAdapter<Category>
    {
        private static SQLiteParameter nameParameter   = new SQLiteParameter("@name", DbType.String, "C_Name");

        protected override string TableName => "Categories";

        protected override SQLiteParameter[] DataParameters => new SQLiteParameter[] { nameParameter };

        /// <summary>
        /// Create a new adapter with the application default connection string
        /// </summary>
        public CategoriesAdapter() : base() { }

        /// <summary>
        /// Create a new adapter with the specified connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public CategoriesAdapter(string connectionString) : base(connectionString) { }

        /// <inheritdoc/>
        protected override void SetDataParametersFromRow(Category row)
        {
            nameParameter.Value = row.C_Name;
        }

        /// <inheritdoc/>
        protected override Category GetRowFromReader(SQLiteDataReader reader)
        {
            try
            {
                Category newCategory = new Category()
                {
                    C_ID = reader.GetInt32(0),
                    C_Name = reader.GetString(1)
                };

                return newCategory;
            }
            catch (InvalidCastException e)
            {
                App.LogException(e);
                return null;
            }
        }
    }
}
