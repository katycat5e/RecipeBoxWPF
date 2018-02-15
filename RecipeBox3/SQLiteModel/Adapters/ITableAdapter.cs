using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Interface defining properties for table adapters</summary>
    public interface ITableAdapter
    {
        /// <summary>Name of the table</summary>
        string TableName { get; }

        /// <summary>Name of the table's primary key column</summary>
        string IDColumn { get; }

        /// <summary>An array of non-identity parameters for insert and update queries</summary>
        IEnumerable<string> DataColumns { get; }
    }
}
