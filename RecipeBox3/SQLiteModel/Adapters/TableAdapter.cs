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
        public TableColumn IDColumn => new TableColumn(
            IDColumnName,
            System.Data.DbType.Int32,
            -1,
            ColumnOptions.NotNull | ColumnOptions.PrimaryKey
        );

        /// <summary>Name of the table's primary key column</summary>
        public virtual string IDColumnName => "ID";

        /// <summary>Columns in this table</summary>
        public abstract IEnumerable<TableColumn> DataColumns { get; }

        /// <summary>All of the columns in this table</summary>
        public virtual List<TableColumn> Columns
        {
            get
            {
                IEnumerable<TableColumn> datacolumns = DataColumns;
                var columns = new List<TableColumn>(datacolumns.Count() + 1) { IDColumn };
                columns.AddRange(datacolumns);
                return columns;
            }
        }

        /// <summary>Names of non-identity columns for insert and update queries</summary>
        public IEnumerable<string> DataColumnNames => DataColumns.Select(c => c.ColumnName).ToList();
    }
}
