using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Abstract adapter for a table in the database</summary>
    /// <typeparam name="T">Type of row objects held in this table</typeparam>
    public abstract class SQLiteAdapter<T> where T : CookbookRow<T>
    {
        /// <summary>Connection string for this adapter</summary>
        protected string _connectionString;

        /// <summary>Get/Set the connection string for this adapter</summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                _connectionString = value;
                Connection.ConnectionString = value;
            }
        }

        /// <summary>Connection for this adapter</summary>
        protected SQLiteConnection _connection;

        /// <summary>Get/set the connection for this adapter</summary>
        protected virtual SQLiteConnection Connection
        {
            get
            {
                if (_connection == null) Connection = new SQLiteConnection(ConnectionString);
                return _connection;
            }
            set
            {
                if (value == _connection) return;
                else
                {
                    _connection = value;
                    if (SelectCommand != null) SelectCommand.Connection = _connection;
                    if (InsertCommand != null) InsertCommand.Connection = _connection;
                    if (UpdateCommand != null) UpdateCommand.Connection = _connection;
                    if (DeleteCommand != null) DeleteCommand.Connection = _connection;
                    if (LastIDCommand != null) LastIDCommand.Connection = _connection;
                }
            }
        }

        /// <summary>Parameter for the table's primary key column</summary>
        protected SQLiteParameter IDParameter = new SQLiteParameter("@id", DbType.Int32);

        /// <summary>An array of non-identity parameters for insert and update queries</summary>
        protected abstract SQLiteParameter[] DataParameters { get; }

        /// <summary>Command to select one (by id) or all rows from the table</summary>
        protected SQLiteCommand SelectCommand;
        /// <summary>Command to insert a new row</summary>
        protected SQLiteCommand InsertCommand;
        /// <summary>Command to update an existing row</summary>
        protected SQLiteCommand UpdateCommand;
        /// <summary>Command to delete a row</summary>
        protected SQLiteCommand DeleteCommand;
        /// <summary>Command to fetch the primary key ID of the last row inserted into the table</summary>
        protected SQLiteCommand LastIDCommand;

        /// <summary>Name of the table this adapter manages</summary>
        protected abstract string TableName { get; }

        /// <summary>Name of the primary key ID column for this adapter's table</summary>
        protected virtual string IDColumn => IDParameter.SourceColumn;

        /// <summary>Collection of this adapter's table's data column names</summary>
        protected virtual IEnumerable<string> DataColumns =>
            DataParameters.Select(p => p.SourceColumn).ToList();

        /// <summary>Collection of this adapter's parameter names</summary>
        protected virtual IEnumerable<string> DataParamNames =>
            DataParameters.Select(p => p.ParameterName).ToList();

        /// <summary>
        /// Value of the last autoincrement primary key inserted into this table
        /// </summary>
        public virtual int? LastInsertedID
        {
            get
            {
                if (LastIDCommand?.Connection != null)
                {
                    var id = ExecuteCommandScalar(LastIDCommand);

                    try
                    {
                        return Convert.ToInt32(id);
                    }
                    catch (Exception e)
                    {
                        App.LogException(e);
                        return null;
                    }
                }
                else return null;
            }
        }

        /// <summary>Create a new instance of the class</summary>
        protected SQLiteAdapter() : this(Properties.Settings.Default.SQLiteConnectionString) { }

        /// <summary>Create a new instance of the class with the specified connection</summary>
        protected SQLiteAdapter(string connectionString)
        {
            connectionString = Environment.ExpandEnvironmentVariables(connectionString);
            Initialize(connectionString);
        }

        /// <summary>Setup the primary table commands</summary>
        /// <param name="connectionString">Connection string to be used for the connection</param>
        protected virtual void Initialize(string connectionString)
        {
            _connectionString = connectionString;

            // Setup Select command
            SelectCommand = new SQLiteCommand()
            {
                CommandText = String.Format(
                    "SELECT {0}, {1} FROM {2} WHERE (@id IS NULL) OR ({0} = @id)",
                    IDColumn,
                    String.Join(", ", DataColumns),
                    TableName)
            };

            SelectCommand.Parameters.Add(IDParameter);

            // Setup Insert command
            InsertCommand = new SQLiteCommand()
            {
                CommandText = String.Format(
                    "INSERT INTO {0} ({1}) VALUES ({2})",
                    TableName,
                    String.Join(", ", DataColumns),
                    String.Join(", ", DataParamNames))
            };

            InsertCommand.Parameters.Add(IDParameter);
            InsertCommand.Parameters.AddRange(DataParameters);

            // Setup Update command
            UpdateCommand = new SQLiteCommand();

            var columnAssignments = DataParameters.Select(p => String.Format("{0}={1}", p.SourceColumn, p.ParameterName));
            
            UpdateCommand.CommandText = String.Format("UPDATE {0} SET {1} WHERE ({2}=@id)",
                TableName,
                String.Join(", ", columnAssignments),
                IDColumn);

            UpdateCommand.Parameters.Add(IDParameter);
            UpdateCommand.Parameters.AddRange(DataParameters);

            // Setup Delete Command
            DeleteCommand = new SQLiteCommand()
            {
                CommandText = String.Format(
                    "DELETE FROM {0} WHERE ({1} = @id)",
                    TableName,
                    IDColumn)
            };

            DeleteCommand.Parameters.Add(IDParameter);

            LastIDCommand = new SQLiteCommand()
            {
                CommandText = String.Format(
                    "SELECT seq FROM sqlite_sequence WHERE name='{0}'",
                    TableName)
            };

            Connection = new SQLiteConnection(_connectionString);
        }

        /// <summary>Fetch all rows from the table</summary>
        /// <returns>Enumerable set of row objects from the table</returns>
        public virtual IEnumerable<T> SelectAll()
        {
            if (SelectCommand.Connection == null) return null;
            else
            {
                List<T> results = new List<T>();
                IDParameter.Value = null;

                using (var reader = ExecuteCommandReader(SelectCommand))
                {
                    if (reader.HasRows)
                    {
                        T nextRow;

                        while (reader.Read())
                        {
                            nextRow = GetRowFromReader(reader);
                            if (nextRow != null) results.Add(nextRow);
                        }
                    }
                }

                return results;
            }
        }

        /// <summary>Try to fetch a single row from the table by its ID</summary>
        /// <param name="id">Primary key ID of the row to look for</param>
        /// <returns>The row object with the desired ID, or null if no such row exists</returns>
        public virtual T Select(int id)
        {
            if (SelectCommand.Connection == null) return null;
            else
            {
                IDParameter.Value = id;
                T row = null;

                using (var reader = ExecuteCommandReader(SelectCommand))
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        row = GetRowFromReader(reader);
                    }
                }

                return row;
            }
        }

        /// <summary>Construct a row object using data returned from the database</summary>
        /// <param name="reader">DataReader to pull values from</param>
        /// <returns>Row object representing the current row from the reader</returns>
        protected abstract T GetRowFromReader(SQLiteDataReader reader);

        /// <summary>
        /// Set all data parameters other than the row's primary key ID
        /// </summary>
        /// <param name="row">Row object to pull values from</param>
        protected abstract void SetDataParametersFromRow(T row);
        
        /// <summary>
        /// Perform an update command on a row in the database
        /// </summary>
        /// <param name="row">row containing new values and the id of a row in the db</param>
        /// <returns>true if one or more rows were updated successfully</returns>
        public virtual bool Modify(T row)
        {
            IDParameter.Value = row.ID;
            SetDataParametersFromRow(row);
            return (ExecuteCommandNonQuery(UpdateCommand) > 0);
        }
        
        /// <summary>
        /// Insert a row into the database
        /// </summary>
        /// <param name="row">row to be inserted</param>
        /// <returns>true if the row was inserted successfully</returns>
        public virtual bool Insert(T row)
        {
            SetDataParametersFromRow(row);
            return (ExecuteCommandNonQuery(InsertCommand) > 0);
        }

        /// <summary>
        /// Delete a row from the database by id
        /// </summary>
        /// <param name="id">id to match in <see cref="IDColumn"/></param>
        /// <returns>true if one or more rows were deleted</returns>
        public virtual bool Delete(int? id)
        {
            if (id.HasValue)
            {
                IDParameter.Value = id.Value;
                return (ExecuteCommandNonQuery(DeleteCommand) > 0);
            }
            else return false;
        }

        /// <summary>
        /// Alias for Delete(row.ID)
        /// </summary>
        /// <param name="row">row to delete from db</param>
        /// <returns>true if one or more rows were deleted</returns>
        public virtual bool Delete(T row)
        {
            return Delete(row?.ID);
        }

        /// <summary>Save changes from a row to the database if necessary</summary>
        /// <param name="row">Row object to be saved</param>
        /// <returns>true if modifications were made to the database</returns>
        public bool Update(T row)
        {
            switch (row.Status)
            {
                case RowStatus.New:
                    return Insert(row);

                case RowStatus.Modified:
                    return Modify(row);

                case RowStatus.Deleted:
                    return Delete(row);

                case RowStatus.Unchanged:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Update each row object in an enumerable set
        /// </summary>
        /// <param name="rows">set of rows to update</param>
        public int Update(IEnumerable<T> rows)
        {
            int rowsAffected = 0;

            foreach (T row in rows)
            {
                if (Update(row)) rowsAffected += 1;
            }

            return rowsAffected;
        }

        /// <summary>
        /// Execute a non-reader <see cref="SQLiteCommand"/>, returning the number of rows affected
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns>Number of rows affected</returns>
        protected static int ExecuteCommandNonQuery(SQLiteCommand command)
        {
            command.Connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            command.Connection.Close();
            return rowsAffected;
        }

        /// <summary>
        /// Execute an <see cref="SQLiteCommand"/> and return a reader
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns>DataReader containing results for the query</returns>
        protected static SQLiteDataReader ExecuteCommandReader(SQLiteCommand command)
        {
            command.Connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Execute an <see cref="SQLiteCommand"/> and returns a scalar value
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns>object from the first column of the first row in the result set</returns>
        protected static object ExecuteCommandScalar(SQLiteCommand command)
        {
            command.Connection.Open();
            object result = command.ExecuteScalar();
            command.Connection.Close();
            return result;
        }
    }
}
