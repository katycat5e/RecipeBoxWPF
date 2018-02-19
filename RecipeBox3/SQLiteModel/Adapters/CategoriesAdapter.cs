using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Adapter for the Categories table</summary>
    public sealed class CategoriesAdapter : SQLiteAdapter<Category>
    {
        /// <inheritdoc/>
        public override string TableName => "Categories";
        /// <inheritdoc/>
        public override string IDColumnName => "C_ID";
        /// <inheritdoc/>
        public override IEnumerable<TableColumn> DataColumns => new TableColumn[]
        {
            new TableColumn("C_Name", DbType.String, "New Category", true, true),
            new TableColumn("C_Editable", DbType.Boolean, true)
        };

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
            TrySetParameterValue("C_Name", row.C_Name);
            TrySetParameterValue("C_Editable", row.IsUserEditable);
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
