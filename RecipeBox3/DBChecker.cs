using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SQLite;
using RecipeBox3.SQLiteModel.Adapters;
using System.Windows;
using System.IO;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3
{
    /// <summary>Class for checking and updating the database structure</summary>
    public class DBChecker : DependencyObject
    {
        private static readonly string CURRENT_DB_VERSION = "3.0.0";

        private const string UPGRADE_DB_PROMPT =
            "The current database (version ~VERSION~) is not compatible with this version of RecipeBox. " +
            "To continue with this version of RecipeBox, the database must be upgraded to version ~NEWVERSION~.\n\n" +
            "Selecting 'Yes' will backup the existing database and attempt the upgrade\n\n" +
            "Selecting 'No' will leave the data unaffected and quit the appliction.";

        private SQLiteConnection Connection;
        SQLiteCommand CheckTableExistsCommand;
        SQLiteCommand GetColumnsCommand;
        SQLiteCommand GetIndexesCommand;
        SQLiteCommand GetIndexColumnCommand;

        SQLiteParameter TableNameParameter = new SQLiteParameter("@tableName", DbType.String);
        SQLiteParameter IndexNameParameter = new SQLiteParameter("@indexName", DbType.String);

        PropertiesAdapter propertiesAdapter;

        /// <summary>Create a new instance of the class</summary>
        /// <param name="connectionString"></param>
        public DBChecker(string connectionString)
        {
            Connection = new SQLiteConnection(connectionString);

            CheckTableExistsCommand = new SQLiteCommand(
                "SELECT count(*) FROM `sqlite_master` WHERE type='table' AND name=@tableName",
                Connection);
            CheckTableExistsCommand.Parameters.Add(TableNameParameter);

            GetColumnsCommand = new SQLiteCommand(
                "SELECT `name`, `type`, `notnull`, `dflt_value`, `pk` FROM pragma_table_info(@tableName)",
                Connection);
            GetColumnsCommand.Parameters.Add(TableNameParameter);

            GetIndexesCommand = new SQLiteCommand(
                "SELECT `name` FROM pragma_index_list(@tableName) WHERE `unique`=1",
                Connection);
            GetIndexesCommand.Parameters.Add(TableNameParameter);

            GetIndexColumnCommand = new SQLiteCommand(
                "SELECT `name` FROM pragma_index_info(@indexName)", Connection);
            GetIndexColumnCommand.Parameters.Add(IndexNameParameter);

            propertiesAdapter = new PropertiesAdapter(connectionString);
        }

        /// <summary>Perform an examination of the database structure and attempt to update if necesssary</summary>
        /// <returns>
        /// True if the database is already up to date or was upgraded successfully,
        /// false if the database could not be verified as compatible
        /// </returns>
        public bool CheckDatabase()
        {
            try
            {
                Connection.Open();

                CurrentOperation = "Checking database version...";
                ShowProgress = false;

                PropertyRow dbVersion = null;

                try
                {
                    dbVersion = GetDBVersion();
                }
                catch (Exception) { }

                if (dbVersion == null) dbVersion = new PropertyRow("Version", "none");

                if (dbVersion?.Value == CURRENT_DB_VERSION)
                {
                    // Database is current
                    Connection.Close();
                    return true;
                }
                else
                {
                    // Needs upgrade
                    var result = MessageBox.Show(
                        UPGRADE_DB_PROMPT.Replace("~VERSION~", dbVersion.Value).Replace("~NEWVERSION~", CURRENT_DB_VERSION),
                        "Database Error",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.Yes);

                    bool upgradeSuccessful = false;
                    if (result == MessageBoxResult.Yes) upgradeSuccessful = UpgradeDatabase();
                    
                    Connection.Close();

                    if (upgradeSuccessful)
                    {
                        dbVersion.Value = CURRENT_DB_VERSION;
                        propertiesAdapter.Update(dbVersion);
                    }

                    return upgradeSuccessful;
                }
            }
            catch (Exception ex)
            {
                App.LogException(ex);
                
                if (Connection.State != ConnectionState.Closed) Connection.Close();

                MessageBox.Show(
                    "An error occurred while checking the database:\n\n" + ex.Message,
                    "Error Checking Database", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
        }

        /// <summary>Iterate through the database and update the table structure</summary>
        /// <returns>True if the update completed successfully</returns>
        protected bool UpgradeDatabase()
        {
            var backupDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\Backups\\");
            string backupDBFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups\\backup_" + DateTime.Now.ToFileTime() + ".db");

            try
            {
                CurrentOperation = "Backing up data...";
                ShowProgress = false;

                Directory.CreateDirectory(backupDir.FullName);
                foreach (FileInfo f in backupDir.GetFiles())
                {
                    try
                    {
                        f.Delete();
                    }
                    catch (Exception) { }
                }

                var backupConnection = new SQLiteConnection($"Data Source={backupDBFile};Version=3");

                backupConnection.Open();
                Connection.BackupDatabase(backupConnection, "main", "main", -1, null, 0);
                backupConnection.Close();

                CurrentOperation = "Upgrading database structure...";
                ShowProgress = true;
                OperationProgress = 0;

                int currentIndex = 0;
                int numAdapters = RequiredAdapterTypes.Length;

                foreach (Type adapterType in RequiredAdapterTypes)
                {
                    TableAdapter tableAdapter = Activator.CreateInstance(adapterType) as TableAdapter;
                    
                    string tableName = tableAdapter.TableName;
                    var adapterColumns = tableAdapter.Columns;

                    if (TableExistsInDatabase(tableName))
                    {
                        // table is extant
                        Dictionary<string, TableInfoRow> dbTableColumns = GetTableInfo(tableName);

                        bool columnsMatch = true;
                        for (int i = 0; i < adapterColumns.Count && columnsMatch; i++)
                        {
                            TableColumn requiredColumn = adapterColumns[i];
                            if (dbTableColumns.TryGetValue(requiredColumn.ColumnName, out TableInfoRow dbColumn))
                            {
                                columnsMatch &= dbColumn.Matches(requiredColumn);
                            }
                            else columnsMatch = false;
                        }

                        if (!columnsMatch)
                        {
                            // tables don't match, attempt to recreate the desired table & migrate the data
                            var matchingColumnNames = adapterColumns
                                .Where(
                                    column => dbTableColumns.ContainsKey(column.ColumnName))
                                .Select(
                                    column => column.ColumnName);

                            string backupTableName = tableName + "_backup";

                            RenameTable(tableName, backupTableName);
                            CreateTable(tableName, adapterColumns);
                            CopyTableData(backupTableName, tableName, matchingColumnNames);
                            DropTable(backupTableName);
                        }
                    }
                    else
                    {
                        // no existe
                        CreateTable(tableName, adapterColumns);
                    }

                    currentIndex += 1;
                    OperationProgress = currentIndex * 100 / numAdapters;
                }
            }
            catch (SQLiteException ex)
            {
                App.LogException(ex);

                MessageBox.Show(
                    "An error occurred while upgrading the database:\n\n" + ex.Message +
                    "\n\nThe database will be restored to its previous state",
                    "Upgrade Error", MessageBoxButton.OK, MessageBoxImage.Error);

                try
                {
                    // Restore backup
                    if (File.Exists(backupDBFile) && File.Exists(Connection.FileName))
                    {
                        new FileInfo(Connection.DataSource).Delete();
                        new FileInfo(backupDBFile).MoveTo(Connection.DataSource);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        "Something went horribly wrong and the backup could not be restored",
                        "Restore Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return false;
            }

            return true;
        }

        /// <summary>Check whether the specified table exists in the database</summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected bool TableExistsInDatabase(string tableName)
        {
            TableNameParameter.Value = tableName;

            var result = Convert.ToInt32(CheckTableExistsCommand.ExecuteScalar());
            return (result > 0);
        }

        /// <summary>Get the column definitions for the specified table</summary>
        /// <param name="tableName"></param>
        /// <returns>A Dictionary of the table columns keyed by name</returns>
        protected Dictionary<string, TableInfoRow> GetTableInfo(string tableName)
        {
            var results = new Dictionary<string, TableInfoRow>();
            TableInfoRow tempRow;

            TableNameParameter.Value = tableName;

            var uniqueColumns = GetTableUniqueColumns();

            using (var reader = GetColumnsCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    // table exists, get the columns
                    while (reader.Read())
                    {
                        tempRow = new TableInfoRow
                        {
                            Name = reader.GetString(0),
                            DataType = reader.GetString(1),
                            NotNull = reader.GetBoolean(2),
                            PrimaryKey = reader.GetBoolean(4)
                        };

                        object defaultValue = reader.GetValue(3);
                        if (defaultValue == DBNull.Value)
                            tempRow.Default = null;
                        else
                            tempRow.Default = defaultValue.ToString();

                        tempRow.Unique = uniqueColumns.Contains(tempRow.Name);

                        results.Add(tempRow.Name, tempRow);
                    }
                }
            }
            
            

            return results;
        }

        private List<string> GetTableUniqueColumns()
        {
            var indices = new List<string>();
            using (var indexReader = GetIndexesCommand.ExecuteReader())
            {
                if (indexReader.HasRows)
                {
                    while (indexReader.Read())
                    {
                        indices.Add(indexReader.GetString(0));
                    }
                }
            }

            var results = new List<string>(indices.Count);

            foreach (string index in indices)
            {
                IndexNameParameter.Value = index;
                string indexColumn = Convert.ToString(GetIndexColumnCommand.ExecuteScalar());
                if (indexColumn != null) results.Add(indexColumn);
            }

            return results;
        }

        /// <summary>Create a new table in the database</summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <exception cref="SQLiteException"></exception>
        private void CreateTable(string tableName, IEnumerable<TableColumn> columns)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendFormat("CREATE TABLE `{0}` (", tableName);
            string columnString = String.Join(", ", columns.Select(column => column.GetColumnDeclaration()));
            sb.Append(columnString);
            sb.Append(")");

            var createCommand = new SQLiteCommand(sb.ToString(), Connection);
            createCommand.ExecuteNonQuery();
        }

        /// <summary>Delete a table from the database</summary>
        /// <param name="tableName"></param>
        /// <exception cref="SQLiteException"></exception>
        private void DropTable(string tableName)
        {
            var dropCommand = new SQLiteCommand($"DROP TABLE `{tableName}`", Connection);
            dropCommand.ExecuteNonQuery();
        }

        /// <summary>Rename a table</summary>
        /// <param name="tableName"></param>
        /// <param name="newName"></param>
        /// <exception cref="SQLiteException"></exception>
        private void RenameTable(string tableName, string newName)
        {
            var renameCommand = new SQLiteCommand($"ALTER TABLE `{tableName}` RENAME TO {newName}", Connection);
            renameCommand.ExecuteNonQuery();
        }

        /// <summary>Copy the data in the specified columns from one table to another</summary>
        /// <param name="sourceTableName"></param>
        /// <param name="destTableName"></param>
        /// <param name="columnNames">names of the columns to copy</param>
        /// <exception cref="SQLiteException"></exception>
        private void CopyTableData(string sourceTableName, string destTableName, IEnumerable<string> columnNames)
        {
            string commandText =
                String.Format(
                    "INSERT INTO `{0}` SELECT {1} FROM `{2}`",
                    destTableName,
                    String.Join(", ", columnNames),
                    sourceTableName);
            var copyCommand = new SQLiteCommand(commandText, Connection);
            copyCommand.ExecuteNonQuery();
        }

        private static Type[] RequiredAdapterTypes = new Type[]
        {
            typeof(CategoriesAdapter), typeof(ImagesAdapter), typeof(IngredientsAdapter),
            typeof(RecipesAdapter), typeof(UnitsAdapter), typeof(PropertiesAdapter)
        };

        /// <summary>Get the current version of the database</summary>
        /// <returns></returns>
        public PropertyRow GetDBVersion()
        {
            return propertiesAdapter.SelectByName("Version");
        }

        /// <summary>Description of current operation being performed</summary>
        public string CurrentOperation
        {
            get { return (string)GetValue(CurrentOperationProperty); }
            set { SetValue(CurrentOperationProperty, value); }
        }

        /// <summary>Property store for <see cref='CurrentOperation'/></summary>
        public static readonly DependencyProperty CurrentOperationProperty =
            DependencyProperty.Register("CurrentOperation", typeof(string), typeof(DBChecker), new PropertyMetadata(""));


        /// <summary>Whether the current operation should show its progress</summary>
        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }

        /// <summary>Property store for <see cref='ShowProgress'/></summary>
        public static readonly DependencyProperty ShowProgressProperty =
            DependencyProperty.Register("ShowProgress", typeof(bool), typeof(DBChecker), new PropertyMetadata(false));


        /// <summary>% completion of current upgrade operation</summary>
        public int OperationProgress
        {
            get { return (int)GetValue(OperationProgressProperty); }
            set { SetValue(OperationProgressProperty, value); }
        }

        /// <summary>Property store for <see cref='OperationProgress'/></summary>
        public static readonly DependencyProperty OperationProgressProperty =
            DependencyProperty.Register("UpgradeProgress", typeof(int), typeof(DBChecker), new PropertyMetadata(0));



        /// <summary>Row returned by pragma_table_info()</summary>
        public struct TableInfoRow
        {
            /// <summary>Column name</summary>
            public string Name;

            /// <summary>Column affinity</summary>
            public string DataType;

            /// <summary>Whether the column can contain NULL values</summary>
            public bool NotNull;

            /// <summary>Default value for this column in string form</summary>
            public string Default;

            /// <summary>Whether this column has a unique constraint</summary>
            public bool Unique;

            /// <summary>Whether this column holds a table's primary key</summary>
            public bool PrimaryKey;

            /// <summary>Determine if a <see cref="TableColumn"/> represents the same column as this row</summary>
            /// <param name="column"></param>
            /// <returns></returns>
            public bool Matches(TableColumn column)
            {
                bool columnsMatch = (column.GetSQLiteAffinity() == DataType);
                columnsMatch &= (column.NotNull == NotNull);

                if (column.DataType == DbType.Boolean)
                    columnsMatch &= (Convert.ToInt32(column.DefaultValue).ToString() == Default);
                else
                    columnsMatch &= (column.DefaultValue?.ToString() == Default);

                columnsMatch &= (column.Unique == Unique);
                columnsMatch &= (column.PrimaryKey == PrimaryKey);
                return columnsMatch;
            }
        }
    }
}
