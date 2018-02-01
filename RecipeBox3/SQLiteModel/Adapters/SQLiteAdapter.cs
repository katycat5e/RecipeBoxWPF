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
    public abstract class SQLiteAdapter<T> where T : CookbookRow<T>
    {
        protected string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                _connectionString = value;
                Connection.ConnectionString = value;
            }
        }

        protected SQLiteConnection _connection;
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
                }
            }
        }

        protected SQLiteParameter IDParameter = new SQLiteParameter("@id", DbType.Int32);
        /// <summary>An array of non-identity parameters for insert and update queries</summary>
        protected abstract SQLiteParameter[] DataParameters { get; }

        protected SQLiteCommand SelectCommand;
        protected SQLiteCommand InsertCommand;
        protected SQLiteCommand UpdateCommand;
        protected SQLiteCommand DeleteCommand;

        protected abstract string TableName { get; }

        protected virtual string IDColumn => IDParameter.SourceColumn;

        protected virtual IEnumerable<string> DataColumns =>
            DataParameters.Select(p => p.SourceColumn).ToList();

        protected virtual IEnumerable<string> DataParamNames =>
            DataParameters.Select(p => p.ParameterName).ToList();

        protected SQLiteAdapter() : this(Properties.Settings.Default.SQLiteConnectionString) { }

        protected SQLiteAdapter(string connectionString)
        {
            connectionString = Environment.ExpandEnvironmentVariables(connectionString);
            Initialize(connectionString);
        }

        /// <summary>
        /// Setup the basic table commands
        /// </summary>
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

            Connection = new SQLiteConnection(_connectionString);
        }

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

        protected abstract T GetRowFromReader(SQLiteDataReader reader);

        /// <summary>
        /// Set all fields other than the row ID
        /// </summary>
        /// <param name="row"></param>
        protected abstract void SetDataParametersFromRow(T row);
        
        public virtual bool Modify(T row)
        {
            IDParameter.Value = row.ID;
            SetDataParametersFromRow(row);
            return (ExecuteCommandNonQuery(UpdateCommand) > 0);
        }
        
        public virtual bool Insert(T row)
        {
            SetDataParametersFromRow(row);
            return (ExecuteCommandNonQuery(InsertCommand) > 0);
        }

        public virtual bool Delete(int id)
        {
            IDParameter.Value = id;
            return (ExecuteCommandNonQuery(DeleteCommand) > 0);
        }

        public virtual bool Delete(T row)
        {
            IDParameter.Value = row.ID;
            return (ExecuteCommandNonQuery(DeleteCommand) > 0);
        }


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
        /// Update each <see cref="CookbookRow"/> in an enumerable set
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
        /// Execute a <see cref="SQLiteCommand"/> and return a reader
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns>DataReader containing results for the query</returns>
        protected static SQLiteDataReader ExecuteCommandReader(SQLiteCommand command)
        {
            command.Connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }
}
