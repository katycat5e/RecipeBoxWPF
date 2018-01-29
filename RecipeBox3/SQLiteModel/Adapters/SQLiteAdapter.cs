using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    public abstract class SQLiteAdapter<T> where T : CookbookRow
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

        public abstract List<T> Select();

        public abstract T Select(int id);

        /// <summary>
        /// Update a <see cref="CookbookRow"/>
        /// </summary>
        /// <param name="row">Row to update</param>
        /// <returns>True if the row was updated, otherwise false</returns>
        public abstract bool Update(T row);

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
        /// Insert a new <see cref="CookbookRow"/> into the database
        /// </summary>
        /// <param name="row">Row containing values to insert</param>
        /// <returns>True if the row was successfully inserted</returns>
        public abstract bool Insert(T row);

        public int Insert(IEnumerable<T> rows)
        {
            int rowsInserted = 0;

            foreach (T row in rows)
            {
                if (Insert(row)) rowsInserted += 1;
            }

            return rowsInserted;
        }

        public abstract bool Delete(int id);

        public abstract bool Delete(T row);

        public int Delete(IEnumerable<T> rows)
        {
            int rowsDeleted = 0;

            foreach (T row in rows)
            {
                if (Delete(row)) rowsDeleted += 1;
            }

            return rowsDeleted;
        }
    }
}
