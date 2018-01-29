using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    public abstract class SQLiteAdapter
    {
        protected SQLiteConnection _connection;
        protected SQLiteConnection Connection
        {
            get
            {
                if (_connection == null) InitConnection();
                return _connection;
            }
            set
            {
                if (value == _connection) return;
                else
                {
                    _connection = value;
                    if (DataAdapter?.SelectCommand != null) DataAdapter.SelectCommand.Connection = _connection;
                    if (DataAdapter?.InsertCommand != null) DataAdapter.InsertCommand.Connection = _connection;
                    if (DataAdapter?.UpdateCommand != null) DataAdapter.UpdateCommand.Connection = _connection;
                    if (DataAdapter?.DeleteCommand != null) DataAdapter.DeleteCommand.Connection = _connection;
                }
            }
        }

        public SQLiteDataAdapter DataAdapter;

        public bool ClearBeforeFill { get; set; }

        public SQLiteAdapter()
        {

        }

        protected void InitConnection()
        {
            Connection = new SQLiteConnection(Properties.Settings.Default.SQLiteConnectionString);
        }

        protected void InitAdapter()
        {

        }

        public List<T> GetData<T>() where T : CookbookRow
        {
            return null;
        }
    }
}
