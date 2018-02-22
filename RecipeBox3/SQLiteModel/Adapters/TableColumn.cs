using System;
using System.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Struct representing a column in a table</summary>
    public struct TableColumn
    {
        /// <summary>Name of the column</summary>
        public string ColumnName;

        /// <summary>Type of values stored in this column</summary>
        public DbType DataType;

        /// <summary>Whether this column can hold null values</summary>
        public bool NotNull;

        /// <summary>Whether this column can contain duplicate values</summary>
        public bool Unique;

        /// <summary>Default value for new entries</summary>
        public object DefaultValue;

        /// <summary>Whether this column holds a primary key for the table</summary>
        public bool PrimaryKey;

        /// <summary>Create a new column struct</summary>
        /// <param name="name"></param>
        /// <param name="dataType"></param>
        /// <param name="defaultValue"></param>
        /// <param name="options"></param>
        public TableColumn(string name, DbType dataType, object defaultValue = null, ColumnOptions options = ColumnOptions.None)
        {
            ColumnName = name;
            DataType = dataType;
            DefaultValue = defaultValue;
            NotNull = options.HasFlag(ColumnOptions.NotNull);
            Unique = options.HasFlag(ColumnOptions.Unique);
            PrimaryKey = options.HasFlag(ColumnOptions.PrimaryKey);
        }
    }

    /// <summary>Optional modifiers to be applied to a column</summary>
    [Flags]
    public enum ColumnOptions
    {
        /// <summary>No options defined</summary>
        None = 0,

        /// <summary>Column does not accept null values</summary>
        NotNull = 1,

        /// <summary>Column values must be unique</summary>
        Unique = 2,

        /// <summary>Column contains primary keys</summary>
        PrimaryKey = 4
    }

    /// <summary>Extension methods for <see cref="TableColumn"/></summary>
    public static class TableColumnExtensions
    {
        /// <summary></summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string GetParameterName(string columnName)
        {
            return "@" + columnName.ToLower();
        }

        /// <summary>Get the equivalent sqlite affinity for a data type</summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static string GetSQLiteAffinity(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Int32:
                case DbType.Int64:
                    return "INTEGER";

                case DbType.Single:
                case DbType.Double:
                    return "REAL";

                case DbType.Decimal:
                    return "NUMERIC";

                case DbType.String:
                    return "TEXT";

                case DbType.Binary:
                default:
                    return "BLOB";
            }
        }
    }
}
