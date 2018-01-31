using System;
using System.Collections.Generic;
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
        protected SQLiteConnection Connection
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

        protected SQLiteCommand SelectCommand;
        protected SQLiteCommand InsertCommand;
        protected SQLiteCommand UpdateCommand;
        protected SQLiteCommand DeleteCommand;

        public bool ClearBeforeFill { get; set; }

        protected SQLiteAdapter() : this(Properties.Settings.Default.SQLiteConnectionString)
        {
            
        }

        protected SQLiteAdapter(string connectionString)
        {
            connectionString = Environment.ExpandEnvironmentVariables(connectionString);
            Initialize(connectionString);
        }

        protected virtual void Initialize(string connectionString)
        {
            _connectionString = connectionString;

            SelectCommand = new SQLiteCommand();
            InsertCommand = new SQLiteCommand();
            UpdateCommand = new SQLiteCommand();
            DeleteCommand = new SQLiteCommand();

            Connection = new SQLiteConnection(_connectionString);
        }

        public abstract IEnumerable<T> SelectAll();

        public abstract T Select(int id);
        
        public abstract bool Modify(T row);
        
        public abstract bool Insert(T row);

        public abstract bool Delete(int id);

        public abstract bool Delete(T row);


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
        protected int ExecuteCommandNonQuery(SQLiteCommand command)
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
        protected SQLiteDataReader ExecuteCommandReader(SQLiteCommand command)
        {
            command.Connection.Open();
            return command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
    }
}
