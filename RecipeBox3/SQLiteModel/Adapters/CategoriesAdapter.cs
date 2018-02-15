using System;
using System.Data;
using System.Data.SQLite;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Adapter for the Categories table</summary>
    public sealed class CategoriesAdapter : SQLiteAdapter<Category>
    {
        private SQLiteParameter nameParameter   = new SQLiteParameter("@name", DbType.String, "C_Name");
        private SQLiteParameter editableParameter = new SQLiteParameter("@editable", DbType.Boolean, "C_Editable");

        /// <inheritdoc/>
        protected override string TableName => "Categories";
        /// <inheritdoc/>
        protected override string IDColumn => "C_ID";

        /// <inheritdoc/>
        protected override SQLiteParameter[] DataParameters =>
            new SQLiteParameter[] { nameParameter, editableParameter };

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
                    C_Name = reader.GetString(1),
                    IsUserEditable = reader.GetBoolean(2)
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
