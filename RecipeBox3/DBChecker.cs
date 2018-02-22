using System;
using System.Data;
using System.Data.SQLite;
using RecipeBox3.SQLiteModel.Adapters;

namespace RecipeBox3
{
    public class DBChecker
    {
        private static readonly string CURRENT_DB_VERSION = "3.0.0";

        private SQLiteConnection Connection;
        SQLiteCommand GetColumnsCommand;

        SQLiteParameter TableNameParameter = new SQLiteParameter("@tableName", System.Data.DbType.String);

        public DBChecker()
        {
            Connection = new SQLiteConnection(Properties.Settings.Default.SQLiteConnectionString);
            GetColumnsCommand = new SQLiteCommand(
                "SELECT name, type, notnull, dflt_value, pk FROM pragma_table_info(@tableName)",
                Connection);
            GetColumnsCommand.Parameters.Add(TableNameParameter);
        }

        public bool CheckDatabase()
        {
            try
            {
                Connection.Open();

                string dbVersion = GetDBVersion(Connection);
                if (dbVersion == CURRENT_DB_VERSION)
                {
                    // Database is current
                    Connection.Close();
                    return true;
                }
                else
                {
                    // Run upgrade
                    bool upgradeSuccessful = UpgradeDatabase();
                    Connection.Close();
                    return upgradeSuccessful;
                }
            }
            catch (Exception ex)
            {
                App.LogException(ex);

                if (Connection.State != System.Data.ConnectionState.Closed) Connection.Close();

                return false;
            }
        }

        private bool UpgradeDatabase()
        {
            foreach (Type adapterType in RequiredAdapterTypes)
            {
                TableAdapter tableAdapter = Activator.CreateInstance(adapterType) as TableAdapter;
                var dataColumns = tableAdapter.DataColumnNames;

                string tableName = tableAdapter.TableName;
                TableNameParameter.Value = tableName;

                using (var reader = GetColumnsCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        // table exists, verify the columns

                    }
                }
            }

            return true;
        }

        private static Type[] RequiredAdapterTypes = new Type[]
        {
            typeof(CategoriesAdapter), typeof(ImagesAdapter), typeof(IngredientsAdapter),
            typeof(RecipesAdapter), typeof(UnitsAdapter)
        };

        private static string GetDBVersion(SQLiteConnection connection)
        {
            var command = new SQLiteCommand("SELECT `Value` FROM `Properties` WHERE `Name`='version'", connection);
            return command.ExecuteScalar() as string;
        }

        private const string DB_CONNECT_ERROR =
            "A connection to the database could not be opened.\n\nError message:\n\n";

        private const string DB_CREATE_ERROR =
            "Database connection succeeded, but the cookbook schema could not be created\n\nError message:\n\n";
    }
}
