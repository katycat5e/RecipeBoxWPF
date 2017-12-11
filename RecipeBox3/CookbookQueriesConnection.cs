using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox3.CookbookDataSetTableAdapters
{
    partial class QueriesTableAdapter
    {
        public void SetConnectionString(string connString)
        {
            foreach (IDbCommand command in CommandCollection)
            {
                command.Connection.ConnectionString = connString;
            }
        }
    }
}
