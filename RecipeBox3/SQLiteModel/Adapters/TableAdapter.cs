using System.Collections.Generic;
using System.Linq;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Abstract class defining properties for table adapters</summary>
    public abstract class TableAdapter
    {
        /// <summary>Name of the table</summary>
        public abstract string TableName { get; }

        /// <summary>Identity column for the table</summary>
        public TableColumn IDColumn => new TableColumn()
        {
            ColumnName = IDColumnName,
            DataType = System.Data.DbType.Int32,
            NotNull = true,
            DefaultValue = -1,
            PrimaryKey = true
        };

        /// <summary>Name of the table's primary key column</summary>
        public virtual string IDColumnName => "ID";

        /// <summary>Columns in this table</summary>
        public abstract IEnumerable<TableColumn> DataColumns { get; }

        /// <summary>Names of non-identity columns for insert and update queries</summary>
        public IEnumerable<string> DataColumnNames => DataColumns.Select(c => c.ColumnName).ToList();
    }
}
