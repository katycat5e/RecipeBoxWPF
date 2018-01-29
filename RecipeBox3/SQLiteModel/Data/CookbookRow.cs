using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox3.SQLiteModel.Data
{
    public enum RowStatus
    {
        Unchanged,
        New,
        Modified,
    }

    public abstract class CookbookRow
    {
        public RowStatus Status = RowStatus.Unchanged;

        public delegate void RowChangedHandler(string property);

        protected virtual void OnRowChanged()
        {
            Status = RowStatus.Modified;
        }
    }
}
