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
        private SQLiteParameter nameParameter   = new SQLiteParameter("@name", DbType.String);

        /// <summary>
        /// Create a new adapter with the application default connection string
        /// </summary>
        public CategoriesAdapter() : base() { }

        /// <summary>
        /// Create a new adapter with the specified connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public CategoriesAdapter(string connectionString) : base(connectionString) { }

        /// <summary>
        /// Called from base constructor
        /// </summary>
        /// <param name="connectionString"></param>
        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);

            SelectCommand.CommandText = "SELECT `C_ID`, `C_Name` FROM `Categories` WHERE (@id IS NULL) OR (`C_ID`=@id)";
            SelectCommand.Parameters.Add(idParameter);

            UpdateCommand.CommandText = "UPDATE `Categories` SET `C_Name`=@name WHERE `C_ID`=@id";
            UpdateCommand.Parameters.Add(idParameter);
            UpdateCommand.Parameters.Add(nameParameter);

            InsertCommand.CommandText = "INSERT INTO `Categories` (`C_Name`) VALUES (@name)";
            InsertCommand.Parameters.Add(nameParameter);

            DeleteCommand.CommandText = "DELETE FROM `Categories` WHERE `C_ID`=@id";
            DeleteCommand.Parameters.Add(idParameter);
        }

        protected override void SetDataParametersFromRow(Category row)
        {
            nameParameter.Value = row.C_Name;
        }

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
                Console.WriteLine(e.Message + " at " + e.TargetSite);
                return null;
            }
        }
    }
}
