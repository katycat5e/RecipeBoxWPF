using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3
{
    partial class App
    {
        private static readonly string CURRENT_DB_VERSION = "3.0.0";

        private static bool CheckDatabase()
        {
            try
            {
                var connection = new SQLiteConnection(RecipeBox3.Properties.Settings.Default.SQLiteConnectionString);
                connection.Open();

                string dbVersion = GetDBVersion(connection);
                if (dbVersion == CURRENT_DB_VERSION)
                {
                    // Database is current
                    connection.Close();
                    return true;
                }
                else
                {
                    // Run upgrade
                    bool upgradeSuccessful = UpgradeDatabase(connection);
                    connection.Close();
                    return upgradeSuccessful;
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private static bool UpgradeDatabase(SQLiteConnection connection)
        {
            foreach (Type adapterType in RequiredAdapterTypes)
            {
                ITableAdapter tableAdapter = Activator.CreateInstance(adapterType) as ITableAdapter;
                var dataColumns = tableAdapter.DataColumns;
                
            }
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
