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
        private SQLiteParameter idParameter;
        private SQLiteParameter nameParameter;

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

            idParameter = new SQLiteParameter("@id", DbType.Int32);
            nameParameter = new SQLiteParameter("@name", DbType.String);

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

        /// <summary>Retrieve a list of <see cref="Category"/> objects from the Categories table</summary>
        /// <returns>List containing data from database or null if there is no connection</returns>
        public override List<Category> Select()
        {
            if (SelectCommand?.Connection == null) return null;
            else
            {
                List<Category> results = new List<Category>();
                idParameter.Value = null;

                using (var reader = SelectCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Category nextRow;

                        while (reader.Read())
                        {
                            try
                            {
                                nextRow = new Category(reader.GetInt32(0), reader.GetString(1));
                                results.Add(nextRow);
                            }
                            catch (InvalidCastException) { }
                        }
                    }
                }

                return results;
            }
        }

        /// <summary>
        /// Get a specific <see cref="Category"/> by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Category Select(int id)
        {
            if (SelectCommand?.Connection == null) return null;
            else
            {
                idParameter.Value = id;
                Category row = null;

                using (var reader = SelectCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        try
                        {
                            row = new Category(reader.GetInt32(0), reader.GetString(1));
                        }
                        catch (InvalidCastException) { }
                    }
                }

                return row;
            }
        }

        /// <summary>
        /// Update a <see cref="Category"/> in the database
        /// </summary>
        /// <param name="row"><see cref="Category"/> to update</param>
        /// <returns>True if the row was updated successfully</returns>
        public override bool Update(Category row)
        {
            idParameter.Value = row.C_ID;
            nameParameter.Value = row.C_Name;
            
            return (UpdateCommand.ExecuteNonQuery() > 0);
        }

        /// <summary>
        /// Insert a new <see cref="Category"/> into the database
        /// </summary>
        /// <param name="row"><see cref="Category"/> containing values to insert</param>
        /// <returns>True if the row was inserted successfully</returns>
        public override bool Insert(Category row)
        {
            nameParameter.Value = row.C_Name;

            return (InsertCommand.ExecuteNonQuery() > 0);
        }

        /// <summary>
        /// Delete a <see cref="Category"/> from the database
        /// </summary>
        /// <param name="row"><see cref="Category"/> to delete</param>
        /// <returns>True if the row was deleted successfully</returns>
        public override bool Delete(Category row)
        {
            idParameter.Value = row.C_ID;

            return (DeleteCommand.ExecuteNonQuery() > 0);
        }

        /// <summary>
        /// Delete a <see cref="Category"/> by id
        /// </summary>
        /// <param name="id">C_ID of <see cref="Category"/> to delete</param>
        /// <returns>True if the row was deleted successfully</returns>
        public override bool Delete(int id)
        {
            idParameter.Value = id;

            return (DeleteCommand.ExecuteNonQuery() > 0);
        }
    }
}
